using System.Text;
using System.Text.Json;
using Cinema.DTO;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Cinema.Web.BackgroundServices
{
    public class TicketEmailWorker : BackgroundService
    {
        private readonly ILogger<TicketEmailWorker> _logger;
        private readonly IConfiguration _configuration;
        private IConnection? _connection;
        private IChannel? _channel;

        public TicketEmailWorker(ILogger<TicketEmailWorker> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                var factory = new ConnectionFactory
                {
                    Uri = new Uri(_configuration.GetConnectionString("RabbitMQ") ?? "amqp://guest:guest@localhost:5672/")
                };

                _connection = await factory.CreateConnectionAsync(stoppingToken);
                _channel = await _connection.CreateChannelAsync(cancellationToken: stoppingToken);

                await _channel.QueueDeclareAsync(queue: "ticket_emails",
                                     durable: true,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null, cancellationToken: stoppingToken);

                var consumer = new AsyncEventingBasicConsumer(_channel);
                consumer.ReceivedAsync += async (model, ea) =>
                {
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);
                    
                    try 
                    {
                        var ticketInfo = JsonSerializer.Deserialize<TicketPurchasedEvent>(message);
                        if (ticketInfo != null)
                        {
                            _logger.LogInformation("Processing email for MaHD: {MaHD}", ticketInfo.MaHD);
                            
                            // Giả lập xử lý gửi email (SMTP)
                            await Task.Delay(2000, stoppingToken);
                            
                            _logger.LogInformation("Successfully sent e-ticket to Email: {Email}", ticketInfo.Email);
                        }

                        if (_channel != null)
                            await _channel.BasicAckAsync(deliveryTag: ea.DeliveryTag, multiple: false, cancellationToken: stoppingToken);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error processing ticket message. Nack.");
                        if (_channel != null)
                            await _channel.BasicNackAsync(deliveryTag: ea.DeliveryTag, multiple: false, requeue: true, cancellationToken: stoppingToken);
                    }
                };

                if (_channel != null)
                {
                    await _channel.BasicConsumeAsync(queue: "ticket_emails",
                                         autoAck: false,
                                         consumer: consumer, cancellationToken: stoppingToken);
                }
                
                // Giữ worker chạy liên tục
                while (!stoppingToken.IsCancellationRequested)
                {
                    await Task.Delay(1000, stoppingToken);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "TicketEmailWorker encountered an error");
            }
            finally 
            {
                if (_channel != null) await _channel.DisposeAsync();
                if (_connection != null) await _connection.DisposeAsync();
            }
        }
    }
}

