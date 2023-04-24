using System.IO;
using BridgeWater.Data;
using BridgeWater.Models;
using Microsoft.EntityFrameworkCore;
#pragma warning disable

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

        public async Task<bool> RemoveProductAsync(int productId)
        {
            Product? product = await bridgeContext.Product
                .FirstOrDefaultAsync(e => e.Id == productId);

            if(product != null)
            {
                // get all comments
                Post[] posts = await bridgeContext.Post.Where(e => e.ProductId == productId)
                    .ToArrayAsync();

                // all product orders
                Order[] orders = await bridgeContext.Order.Where(e => e.ProductOrderId == productId)
                    .ToArrayAsync();

                // removes comments and orders
                bridgeContext.Post.RemoveRange(posts);
                bridgeContext.Order.RemoveRange(orders);

                File.Delete(product.LogoImage);
                File.Delete(product.PosterImage);

                bridgeContext.Product.Remove(product);
                await bridgeContext.SaveChangesAsync();
                return true;
            }

            return false;
        }

        public async Task<ProductViewModel?> GetProductDetailsAsync(int id)
        {
            Product? product = await bridgeContext.Product
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product != null)
            {
                Category? category = await bridgeContext.Category
                    .FirstOrDefaultAsync(c => c.Id == product.CategoryId);

                PostRatingViewModel[] posts = (from p in await bridgeContext.Post.Where(e => e.ProductId == id && (e.IsDeleted == null || (e.IsDeleted != null && !e.IsDeleted.Value)))
                    .ToListAsync() join a in await bridgeContext.Account.ToListAsync() on p.AccountId equals a.Id
                    select new PostRatingViewModel
                    {
                        id = p.Id,
                        body = p.Body,
                        rating = p.Rating,
                        username = a.Username,
                        accountId = p.AccountId,
                        createdAt = p.CreatedAt
                    }
                ).ToArray();
                double RatingStars = 0;
                
                if(posts.Length > 0)
                {
                    int? stars = posts.Sum(e => e.rating);
                    RatingStars =  (double)stars.Value / posts.Length;
                }

                ProductViewModel productViewModel = new ProductViewModel
                {
                    Id = product.Id,
                    Name = product.Name,
                    Description = product.Description,
                    CategoryId = category!.Id,
                    Category = category!.Name,
                    TechInfo = product.TechInfo,
                    Stars = RatingStars,
                    postRatingViewModels = posts,
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


                for(int k = 0; k < products.Count; k++)
                {
                    Post[] posts = await bridgeContext.Post.Where(e => e.ProductId == products[k].Id && (e.IsDeleted == null || (e.IsDeleted != null && !e.IsDeleted.Value)))
                        .ToArrayAsync();

                    double RatingStars = 0;

                    if (posts.Length > 0)
                    {
                        int? stars = posts.Sum(e => e.Rating);
                        RatingStars = (double)stars.Value / posts.Length;
                    }

                    products[k].Stars = RatingStars;
                }
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

                for (int k = 0; k < products.Count; k++)
                {
                    Post[] posts = await bridgeContext.Post.Where(e => e.ProductId == products[k].Id && (e.IsDeleted == null || (e.IsDeleted != null && !e.IsDeleted.Value)))
                        .ToArrayAsync();

                    double RatingStars = 0;

                    if (posts.Length > 0)
                    {
                        int? stars = posts.Sum(e => e.Rating);
                        RatingStars = (double)stars.Value / posts.Length;
                    }

                    products[k].Stars = RatingStars;
                }
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
