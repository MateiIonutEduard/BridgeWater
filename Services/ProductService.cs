using BridgeWater.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace BridgeWater.Services
{
    public class ProductService
    {
        readonly IMongoCollection<Product> products;

        public ProductService(IOptions<BridgeWaterSettings> bridgeWaterSettings)
        {
            var mongoClient = new MongoClient(bridgeWaterSettings.Value.ConnectionString);
            var mongoDatabase = mongoClient.GetDatabase(bridgeWaterSettings.Value.DatabaseName);
            products = mongoDatabase.GetCollection<Product>(bridgeWaterSettings.Value.ProductCollectionName);
        }

        public async Task<Product> GetProductAsync(string id)
        {
            Product product = await products.Find(p => p.Id == id).FirstOrDefaultAsync();
            return product;
        }

        public async Task CreateProductAsync(Product product) => await products.InsertOneAsync(product);

        public async Task CreateProductsAsync(Product[] productList) => await products.InsertManyAsync(productList);

        public async Task RemoveProductAsync(string id) => await products.DeleteOneAsync(x => x.Id == id);

        public async Task RemoveProductsAsync() => await products.DeleteManyAsync(Builders<Product>.Filter.Empty);
    }
}
