using Aggregation.WebAPI.Modules.Weather.DTOs;
using Aggregation.WebAPI.Statistics;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using System.Diagnostics;

namespace Aggregation.WebAPI.Modules.Weather
{
    public class WeatherService : IWeatherService
    {
        private readonly HttpClient _httpClient;
        private readonly IMemoryCache _cache;
        private readonly IStatisticsService _statsService;
        private readonly string WeatherApiName = "Weather_Api";

        public WeatherService(HttpClient httpClient, IMemoryCache cache, IStatisticsService statsService,
            IOptions<ExternalApiOptions> apiOptions)
        {
            _httpClient = httpClient;
            _cache = cache;
            _statsService = statsService;
            _httpClient.BaseAddress = new Uri(apiOptions.Value.WeatherApiUrl);
        }

        public async Task<WeatherForecastDto?> GetExternalWeatherCurrentAsync(double lat, double lon)
        {
            string cacheKey = FormattableString.Invariant($"Weather_Lat_{lat:F2}_Lon_{lon:F2}");

            if (_cache.TryGetValue(cacheKey, out WeatherForecastDto? cachedWeather))
            {
                _statsService.LogRequest(WeatherApiName, 1);
                return cachedWeather;
            }

            var stopwatch = Stopwatch.StartNew();

            var endpoint = FormattableString.Invariant($"/v1/forecast?latitude={lat}&longitude={lon}&current=temperature_2m&timezone=UTC");
            var response = await _httpClient.GetAsync(endpoint);
            response.EnsureSuccessStatusCode();

            stopwatch.Stop();
            _statsService.LogRequest(WeatherApiName, stopwatch.ElapsedMilliseconds);

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
