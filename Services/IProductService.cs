using BridgeWater.Data;
using BridgeWater.Models;

namespace BridgeWater.Services
{
    public interface IProductService
    {
        Task<Category[]> GetCategoriesAsync();
        Task<ProductResultModel> GetProductsByCategoryAsync(int? categoryId, int? page);
        Task<Product?> GetProductAsync(int id);
    }
}
