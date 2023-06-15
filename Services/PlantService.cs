using BridgeWater.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
#pragma warning disable

namespace BridgeWater.Services
{
    public class PlantService
    {
        readonly IMongoCollection<Plant> products;

        public PlantService(IOptions<BridgeWaterSettings> bridgeWaterSettings)
        {
            var mongoClient = new MongoClient(bridgeWaterSettings.Value.ConnectionString);
            var mongoDatabase = mongoClient.GetDatabase(bridgeWaterSettings.Value.DatabaseName);
            products = mongoDatabase.GetCollection<Plant>(bridgeWaterSettings.Value.PlantCollectionName);
        }

        public async Task<bool> HasPostRatingAsync(int accountId, string plantId)
        {
            Plant? plant = await products.Find(p => p.Id.CompareTo(plantId) == 0)
                .FirstOrDefaultAsync();

            /* plant exists */
            if(plant != null)
            {
                /* have comments with rating */
                if(plant.comments != null)
                {
                    Comment? comment = plant.comments
                        .FirstOrDefault(e => e.accountId == accountId && (e.isDeleted == null || (e.isDeleted != null && !e.isDeleted.Value)));

                    /* it has been found */
                    return comment != null;
                }
            }

            return true;
        }

        public string?[] GetCategories()
        {
            var hash = new HashSet<string>();

            var categories = products.Find(_ => true)
                .ToList().Select(p => p.category);

            foreach(string category in categories)
            {
                if(!hash.Contains(category))
                    hash.Add(category);
            }

            return hash.ToArray();
        }

        public async Task<Plant[]> GetProductsByNameAsync(string? name, string? category)
        {
            List<Plant>? plants = new List<Plant>();

            if(!string.IsNullOrEmpty(name))
            {
                var product = await products.Find(p => p.name.ToLower().StartsWith(name.ToLower()))
                    .ToListAsync();

                // add these products if list is not empty or null
                if (product != null && product.Count > 0)
                    plants.AddRange(product);
            }

            if(!string.IsNullOrEmpty(category))
            {
                // removes from them if require specific category
                plants = plants.Where(p => p.category.CompareTo(category) == 0)
                    .ToList();
            }

            return plants.ToArray();
        }

        public async Task<Plant[]> GetProductsByCategoryAsync(string? category)
        {
            var product = await products.Find(p => p.category.CompareTo(category) == 0)
                    .ToListAsync();

            return product.ToArray();
        }

        public async Task<Plant> GetProductAsync(string id)
        {
            Plant product = await products.Find(p => p.Id == id).FirstOrDefaultAsync();
            return product;
        }

        public async Task CreateProductAsync(Plant product) => await products.InsertOneAsync(product);

        public async Task CreateProductsAsync(Plant[] productList) => await products.InsertManyAsync(productList);

        public async Task RemoveProductAsync(string id) => await products.DeleteOneAsync(x => x.Id == id);

        public async Task RemoveProductsAsync() => await products.DeleteManyAsync(Builders<Plant>.Filter.Empty);
    }
}
