using Aggregation.WebAPI.Modules.Weather.DTOs;
using Microsoft.Extensions.Options;

namespace Aggregation.WebAPI.Modules.Weather
{
    public class WeatherService : IWeatherService
    {
        private readonly HttpClient _httpClient;

        public WeatherService(HttpClient httpClient, IOptions<ExternalApiOptions> apiOptions)
        {
            _httpClient = httpClient;

            var baseUrl = apiOptions.Value.WeatherApiUrl;
            _httpClient.BaseAddress = new Uri(baseUrl);
        }

        public async Task<WeatherForecastDto?> GetExternalWeatherCurrentAsync(double lat, double lon)
        {
            var endpoint = FormattableString.Invariant($"/v1/forecast?latitude={lat}&longitude={lon}&current=temperature_2m&timezone=UTC");

            var response = await _httpClient.GetAsync(endpoint);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<WeatherForecastDto>();
        }
    }
}
