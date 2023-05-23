using BridgeWater.Data;
using BridgeWater.Models;

namespace BridgeWater.Services
{
    public interface IProductService
    {
        Task<Category[]> GetCategoriesAsync();
        Task<bool> CreateProductAsync(ProductModel productModel);
        Task<ProductResultModel> GetProductsByNameAsync(string? name, int? page);
        Task<ProductResultModel> GetProductsByCategoryAsync(int? categoryId, int? page);
        Task<ProductViewModel?> GetProductDetailsAsync(int id);
        Task<bool> RemoveProductAsync(int productId);
        Task<Product?> GetProductAsync(int id);
    }
}
