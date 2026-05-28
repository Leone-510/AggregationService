using Aggregation.WebAPI.Modules.Products.DTOs;
using Aggregation.WebAPI.Statistics;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using System.Diagnostics;

namespace Aggregation.WebAPI.Modules.Products
{
    public class ProductsService : IProductsService
    {
        private readonly HttpClient _httpClient;
        private readonly IMemoryCache _cache;
        private readonly IStatisticsService _statsService;
        private readonly string ProductsApiName = "Products_Api";

        private const string ProductsCacheKey = "Products_Catalog";

        public ProductsService(HttpClient httpClient, IMemoryCache cache, IStatisticsService statsService, 
            IOptions<ExternalApiOptions> apiOptions)
        {
            _httpClient = httpClient;
            _cache = cache;
            _statsService = statsService;
            _httpClient.BaseAddress = new Uri(apiOptions.Value.ProductsApiUrl);
        }

        public async Task<List<ProductDto>> GetExternalProductsAsync()
        {
            if (_cache.TryGetValue(ProductsCacheKey, out List<ProductDto>? cachedProducts))
            {
                _statsService.LogRequest(ProductsApiName, 1);
                return cachedProducts ?? new List<ProductDto>();
            }

            var stopwatch = Stopwatch.StartNew();

            var response = await _httpClient.GetAsync("products");
            response.EnsureSuccessStatusCode();

            stopwatch.Stop();
            _statsService.LogRequest(ProductsApiName, stopwatch.ElapsedMilliseconds);

            var products = await response.Content.ReadFromJsonAsync<List<ProductDto>>()
                ?? new List<ProductDto>();

            var cacheOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromMinutes(2))
                .SetSlidingExpiration(TimeSpan.FromMinutes(1));

            _cache.Set(ProductsCacheKey, products, cacheOptions);

            return products;
        }
    }
}
