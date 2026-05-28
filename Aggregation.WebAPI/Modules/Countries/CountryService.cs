using Aggregation.WebAPI.Modules.Countries.DTOs;
using Aggregation.WebAPI.Statistics;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using System.Diagnostics;

namespace Aggregation.WebAPI.Modules.Countries
{
    public class CountryService : ICountryService
    {
        private readonly HttpClient _httpClient;
        private readonly IMemoryCache _cache;
        private readonly IStatisticsService _statsService;
        private readonly string CountriesApiName = "Countries_Api";

        private const string CountriesCacheKey = "Countries_List";

        public CountryService(HttpClient httpClient, IMemoryCache cache, IStatisticsService statsService,
            IOptions<ExternalApiOptions> apiOptions)
        {
            _httpClient = httpClient;
            _cache = cache;
            _statsService = statsService;
            _httpClient.BaseAddress = new Uri(apiOptions.Value.CountriesApiUrl);
        }

        public async Task<List<CountryDto>> GetExternalCountriesAsync()
        {
            if (_cache.TryGetValue(CountriesCacheKey, out List<CountryDto>? cachedCountries))
            {
                _statsService.LogRequest(CountriesApiName, 1);
                return cachedCountries ?? new List<CountryDto>();
            }

            var stopwatch = Stopwatch.StartNew();

            var response = await _httpClient.GetAsync("v3.1/all?fields=name,capital,currencies");
            response.EnsureSuccessStatusCode();

            stopwatch.Stop();
            _statsService.LogRequest(CountriesApiName, stopwatch.ElapsedMilliseconds);

            var countries = await response.Content.ReadFromJsonAsync<List<CountryDto>>()
                ?? new List<CountryDto>();

            var cacheOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromMinutes(2))
                .SetSlidingExpiration(TimeSpan.FromMinutes(1));

            _cache.Set(CountriesCacheKey, countries, cacheOptions);

            return countries;
        }
    }
}
