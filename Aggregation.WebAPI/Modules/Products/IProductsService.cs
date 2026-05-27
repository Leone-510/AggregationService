using Aggregation.WebAPI.Modules.Products.DTOs;

namespace Aggregation.WebAPI.Modules.Products
{
    public interface IProductsService
    {
        Task<List<ProductDto>> GetExternalProductsAsync();
    }
}
