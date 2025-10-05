using CleanSkies.Models;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace CleanSkies.Services

{
    public class OpenAQApi : IOpenAQApi
    {
        private readonly HttpClient _httpClient;
        private readonly OpenAQSettings _settings;
        private readonly ILogger<OpenAQApi> _logger;

        private const string BBOX_WEST = "-141.000000,41.676555,-96.818145,83.113881";
        private const string BBOX_EAST = "-96.818145,41.676555,-52.636291,83.113881";

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
                // var endpointWest = $"/v3/locations?bbox=-141.000000,41.676555,-96.818145,83.113881&limit=1000";
                // var endpointEast = $"/v3/locations?bbox=-96.818145,41.676555,-52.636291,83.113881&limit=1000";

                var endpointWest = $"/v3/parameters/2/latest?locations_id=2178,233&limit=1000";
                var endpointEast = $"/v3/parameters/2/latest?locations_id=2178,233&limit=1000";

                _logger.LogInformation($"API Key (first 10 chars): {_settings.ApiKey.Substring(0, Math.Min(10, _settings.ApiKey.Length))}...");

                var response1 = await _httpClient.GetAsync(endpointWest);
                var content1 = await response1.Content.ReadAsStringAsync();

                var response2 = await _httpClient.GetAsync(endpointEast);
                var content2 = await response2.Content.ReadAsStringAsync();

                Console.WriteLine($"Requesting: {_httpClient.BaseAddress}locations/{locationId}");

                if (!response1.IsSuccessStatusCode || !response2.IsSuccessStatusCode)
                {
                    _logger.LogError($"API returned {response1.StatusCode}: {content1}");
                    throw new HttpRequestException($"API Error {response1.StatusCode}: {content1}");
                }

                _logger.LogInformation("Successfully fetched location data");
                // Wrap both as JSON properties
                var wrapper = new
                {
                    top = JsonConvert.DeserializeObject(content1),
                    bottom = JsonConvert.DeserializeObject(content2)
                };

                return JsonConvert.SerializeObject(wrapper);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error fetching location {locationId}");
                throw;
            }
        }
    }
}