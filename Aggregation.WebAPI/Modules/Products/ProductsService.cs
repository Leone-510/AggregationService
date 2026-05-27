using Aggregation.WebAPI.Modules.Products.DTOs;
using Microsoft.Extensions.Options;

namespace Aggregation.WebAPI.Modules.Products
{
    public class ProductsService : IProductsService
    {
        private readonly HttpClient _httpClient;

        public ProductsService(HttpClient httpClient, IOptions<ExternalApiOptions> apiOptions)
        {
            _httpClient = httpClient;

            var baseUrl = apiOptions.Value.ProductsApiUrl;
            _httpClient.BaseAddress = new Uri(baseUrl);
        }

        public async Task<List<ProductDto>> GetExternalProductsAsync()
        {
            var response = await _httpClient.GetAsync("products");
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<List<ProductDto>>()
                ?? new List<ProductDto>();
        }
    }
}
