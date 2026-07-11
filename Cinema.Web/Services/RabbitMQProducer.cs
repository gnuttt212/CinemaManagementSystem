using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace Cinema.Web.Services
{
    public interface IMessageProducer
    {
        Task SendMessageAsync<T>(T message, string queueName);
    }

    public class RabbitMQProducer : IMessageProducer, IAsyncDisposable
    {
        private readonly ILogger<RabbitMQProducer> _logger;
        private readonly IConfiguration _configuration;
        private IConnection? _connection;
        private IChannel? _channel;

        public RabbitMQProducer(IConfiguration configuration, ILogger<RabbitMQProducer> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        private async Task InitializeAsync()
        {
            if (_channel != null) return;

            try
            {
                var factory = new ConnectionFactory
                {
                    Uri = new Uri(_configuration.GetConnectionString("RabbitMQ") ?? "amqp://guest:guest@localhost:5672/")
                };

                _connection = await factory.CreateConnectionAsync();
                _channel = await _connection.CreateChannelAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Could not connect to RabbitMQ");
            }
        }

        public async Task SendMessageAsync<T>(T message, string queueName)
        {
            await InitializeAsync();
            if (_channel == null) return;

            try
            {
                await _channel.QueueDeclareAsync(queue: queueName,
                                     durable: true,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                var jsonString = JsonSerializer.Serialize(message);
                var body = Encoding.UTF8.GetBytes(jsonString);

                var properties = new BasicProperties
                {
                    Persistent = true
                };

                await _channel.BasicPublishAsync(exchange: "",
                                     routingKey: queueName,
                                     mandatory: false,
                                     basicProperties: properties,
                                     body: body);
                
                _logger.LogInformation("Sent message to queue {QueueName}", queueName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send message to queue {QueueName}", queueName);
            }
        }

        public async ValueTask DisposeAsync()
        {
            if (_channel != null) await _channel.DisposeAsync();
            if (_connection != null) await _connection.DisposeAsync();
        }
    }
}

