using BridgeWater.Data;
using BridgeWater.Models;
using Microsoft.EntityFrameworkCore;

namespace BridgeWater.Services
{
    public class ProductService : IProductService
    {
        readonly BridgeContext bridgeContext;

        public ProductService(BridgeContext bridgeContext)
        { this.bridgeContext = bridgeContext; }

        public async Task<Category[]> GetCategoriesAsync()
        {
            Category[] categories = await bridgeContext.Category.ToArrayAsync();
            return categories;
        }

        public async Task<ProductViewModel?> GetProductDetailsAsync(int id)
        {
            Product? product = await bridgeContext.Product
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product != null)
            {
                Category? category = await bridgeContext.Category
                    .FirstOrDefaultAsync(c => c.Id == product.CategoryId);

                ProductViewModel productViewModel = new ProductViewModel
                {
                    Id = product.Id,
                    Name = product.Name,
                    Description = product.Description,
                    CategoryId = category!.Id,
                    Category = category!.Name,
                    TechInfo = product.TechInfo,
                    Price = product.Price,
                    Stock = product.Stock
                };

                return productViewModel;
            }

            return null;
        }

        public async Task<Product?> GetProductAsync(int id)
        {
            Product? product = await bridgeContext.Product
                .FirstOrDefaultAsync(p => p.Id == id);

            return product;
        }

        public async Task<ProductResultModel> GetProductsByCategoryAsync(int? categoryId, int? page)
        {
            int index = (page != null && page.Value >= 1) ? page.Value - 1 : 0;
            List<ProductViewModel> products = new List<ProductViewModel>();
            int frames, TotalPages = 1;

            if (categoryId != null)
            {
                frames = await bridgeContext.Product.Where(p => p.CategoryId == categoryId)
                    .CountAsync();

                TotalPages = frames >> 3;
                if ((frames & 0x7) != 0) TotalPages++;


                products = await bridgeContext.Product
                    .Where(p => p.CategoryId == categoryId).Skip(8 * index).Take(8)
                    .Select(p => new ProductViewModel
                    {
                        Id = p.Id,
                        Name = p.Name,
                        Description = p.Description,
                        TechInfo = p.TechInfo,
                        Price = p.Price,
                        Stock = p.Stock
                    }).ToListAsync();
            }
            else
            {
                frames = await bridgeContext.Product.CountAsync();

                TotalPages = frames >> 3;
                if ((frames & 0x7) != 0) TotalPages++;


                products = (await bridgeContext.Product
                    .ToListAsync()).Skip(8 * index).Take(8)
                    .Select(p => new ProductViewModel
                    {
                        Id = p.Id,
                        Name = p.Name,
                        Description = p.Description,
                        TechInfo = p.TechInfo,
                        Price = p.Price,
                        Stock = p.Stock
                    }).ToList();
            }

            ProductResultModel productResultModel = new ProductResultModel
            {
                Pages = TotalPages,
                ProductViewModels = products.ToArray()
            };

            return productResultModel;
        }
    }
}
