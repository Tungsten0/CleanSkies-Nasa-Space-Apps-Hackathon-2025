using CleanSkies.Models;
using Microsoft.Extensions.Options;

namespace CleanSkies.Services

{
    public class OpenAQApi : IOpenAQApi
    {
        private readonly HttpClient _httpClient;
        private readonly OpenAQSettings _settings;
        private readonly ILogger<OpenAQApi> _logger;

        public OpenAQApi(HttpClient httpClient, IOptions<OpenAQSettings> settings, ILogger<OpenAQApi> logger)
        {
            _httpClient = httpClient;
            _settings = settings.Value;
            _logger = logger;

            _httpClient.BaseAddress = new Uri(_settings.BaseUrl);
            _httpClient.DefaultRequestHeaders.Add("X-API-Key", _settings.ApiKey);
            _logger.LogInformation($"OpenAQ configured with BaseUrl: {_settings.BaseUrl}");
        }

        public async Task<string> GetLocationDataAsync(int locationId)
        {
            try
            {
                var endpoint = $"/v3/locations/{locationId}";
                var fullUrl = $"{_httpClient.BaseAddress}{endpoint}";

                _logger.LogInformation($"Requesting: {fullUrl}");
                _logger.LogInformation($"API Key (first 10 chars): {_settings.ApiKey.Substring(0, Math.Min(10, _settings.ApiKey.Length))}...");

                var response = await _httpClient.GetAsync(endpoint);
                var content = await response.Content.ReadAsStringAsync();

                Console.WriteLine($"Requesting: {_httpClient.BaseAddress}locations/{locationId}");

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError($"API returned {response.StatusCode}: {content}");
                    throw new HttpRequestException($"API Error {response.StatusCode}: {content}");
                }

                _logger.LogInformation("Successfully fetched location data");
                return content;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error fetching location {locationId}");
                throw;
            }
        }
    }
}