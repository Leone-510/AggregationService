using Aggregation.WebAPI.Modules.Weather.DTOs;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace Aggregation.WebAPI.Modules.Weather
{
    public class WeatherService : IWeatherService
    {
        private readonly HttpClient _httpClient;
        private readonly IMemoryCache _cache;

        public WeatherService(HttpClient httpClient, IMemoryCache cache, IOptions<ExternalApiOptions> apiOptions)
        {
            _httpClient = httpClient;
            _cache = cache;
            _httpClient.BaseAddress = new Uri(apiOptions.Value.WeatherApiUrl);
        }

        public async Task<WeatherForecastDto?> GetExternalWeatherCurrentAsync(double lat, double lon)
        {
            string cacheKey = FormattableString.Invariant($"Weather_Lat_{lat:F2}_Lon_{lon:F2}");

            if (_cache.TryGetValue(cacheKey, out WeatherForecastDto? cachedWeather))
                return cachedWeather;

            var endpoint = FormattableString.Invariant($"/v1/forecast?latitude={lat}&longitude={lon}&current=temperature_2m&timezone=UTC");
            var response = await _httpClient.GetAsync(endpoint);
            response.EnsureSuccessStatusCode();

            var weather = await response.Content.ReadFromJsonAsync<WeatherForecastDto>();
            if (weather != null)
            {
                var cacheOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromMinutes(2));

                _cache.Set(cacheKey, weather, cacheOptions);
            }

            return weather;
        }
    }
}
