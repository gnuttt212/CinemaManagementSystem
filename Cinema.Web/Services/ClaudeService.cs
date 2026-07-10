using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;

namespace Cinema.Web.Services
{
    public class ClaudeService : IClaudeService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private const string ApiUrl = "https://api.anthropic.com/v1/messages";

        // Inject HttpClient qua IHttpClientFactory (tránh socket exhaustion)
        // và IConfiguration để lấy API key từ appsettings/secrets, không hard-code
        public ClaudeService(HttpClient httpClient, IConfiguration config)
        {
            _httpClient = httpClient;
            _apiKey = config["Claude:ApiKey"]
                ?? throw new InvalidOperationException("Thiếu Claude:ApiKey trong cấu hình");
        }

        public async Task<string> GetCompletionAsync(string userPrompt)
        {
            var requestBody = new
            {
                model = "claude-3-5-sonnet-20241022",
                max_tokens = 1024,
                messages = new[]
                {
                    new { role = "user", content = userPrompt }
                }
            };

            var request = new HttpRequestMessage(HttpMethod.Post, ApiUrl)
            {
                Content = new StringContent(
                    JsonSerializer.Serialize(requestBody),
                    Encoding.UTF8,
                    "application/json")
            };

            // Header bắt buộc theo Anthropic API
            request.Headers.Add("x-api-key", _apiKey);
            request.Headers.Add("anthropic-version", "2023-06-01");

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var responseBody = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(responseBody);

            // Trích xuất text từ content[0].text theo cấu trúc response của Messages API
            return doc.RootElement
                .GetProperty("content")[0]
                .GetProperty("text")
                .GetString() ?? string.Empty;
        }
    }
}
