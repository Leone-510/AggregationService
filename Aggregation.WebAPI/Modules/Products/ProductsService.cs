using Aggregation.WebAPI.Modules.Products.DTOs;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace Aggregation.WebAPI.Modules.Products
{
    public class ProductsService : IProductsService
    {
        private readonly HttpClient _httpClient;
        private readonly IMemoryCache _cache;
        private const string ProductsCacheKey = "Products_Catalog";

        public ProductsService(HttpClient httpClient, IMemoryCache cache, IOptions<ExternalApiOptions> apiOptions)
        {
            _httpClient = httpClient;
            _cache = cache;
            _httpClient.BaseAddress = new Uri(apiOptions.Value.ProductsApiUrl);
        }

        public async Task<List<ProductDto>> GetExternalProductsAsync()
        {
            if (_cache.TryGetValue(ProductsCacheKey, out List<ProductDto>? cachedProducts))
                return cachedProducts ?? new List<ProductDto>();

            var response = await _httpClient.GetAsync("products");
            response.EnsureSuccessStatusCode();

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
