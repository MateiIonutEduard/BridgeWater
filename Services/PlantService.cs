using BridgeWater.Data;
using BridgeWater.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System.Net;
#pragma warning disable

namespace BridgeWater.Services
{
    public class PlantService
    {
        readonly IMongoCollection<Plant> products;
        readonly BridgeContext bridgeContext;

        public PlantService(IOptions<BridgeWaterSettings> bridgeWaterSettings, BridgeContext bridgeContext)
        {
            this.bridgeContext = bridgeContext;
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
                    return comment == null;
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

        public async Task<int> CreatePostCommentAsync(CommentRatingModel commentRatingModel)
        {
            Plant? plant = await products.Find(p => p.Id.CompareTo(commentRatingModel.plantId) == 0)
                .FirstOrDefaultAsync();

            if(plant != null)
            {
                Comment? comment = plant.comments != null ? plant.comments
                    .FirstOrDefault(c => c.accountId == commentRatingModel.accountId && (c.isDeleted != null ? !c.isDeleted.Value : false)) : null;

                if(comment == null)
                {
                    /* check if it is valid */
                    if(!string.IsNullOrEmpty(commentRatingModel.message) && commentRatingModel.rating != null)
                    {
                        comment = new Comment
                        {
                            Id = Guid.NewGuid().ToString(),
                            body = commentRatingModel.message,
                            rating = commentRatingModel.rating,
                            accountId = commentRatingModel.accountId.Value,
                            createdAt = DateTime.UtcNow,
                            isDeleted = false
                        };

                        List<Comment> comments = new List<Comment>();

                        /* add comments to list when not empty */
                        if(plant.comments != null && plant.comments.Length > 0) 
                            comments.AddRange(plant.comments);

                        comments.Add(comment);
                        plant.comments = comments.ToArray();
                        await products.ReplaceOneAsync(e => e.Id.CompareTo(plant.Id) == 0, plant);
                        return 1;
                    }

                    /* not valid */
                    return 0;
                }

                /* comment found */
                return -1;
            }

            /* plant does not exists */
            return -2;
        }

        public async Task<bool> CreateReplyPostAsync(CommentRatingModel commentRatingModel)
        {
            Plant? plant = await products.Find(p => p.Id.CompareTo(commentRatingModel.plantId) == 0)
                .FirstOrDefaultAsync();

            if(plant != null)
            {
                Comment? comment = plant.comments != null ? plant.comments
                    .FirstOrDefault(c => c.accountId == commentRatingModel.accountId && c.replyTo != null && c.replyTo.CompareTo(commentRatingModel.replyTo) == 0 && (c.isDeleted != null ? !c.isDeleted.Value : false)) : null;

                if (comment != null) return false;
                List<Comment> comments = new List<Comment>();

                if(plant.comments != null) 
                    comments.AddRange(plant.comments);

                if (!string.IsNullOrEmpty(commentRatingModel.message))
                {
                    comment = new Comment
                    {
                        Id = Guid.NewGuid().ToString(),
                        accountId = commentRatingModel.accountId.Value,
                        replyTo = commentRatingModel.replyTo,
                        body = commentRatingModel.message,
                        createdAt = DateTime.UtcNow,
                        isDeleted = false
                    };

                    comments.Add(comment);
                    plant.comments = comments.ToArray();

                    /* update plant entity from collection */
                    await products.ReplaceOneAsync(p => p.Id.CompareTo(commentRatingModel.plantId) == 0, plant);
                    return true;
                }

                return false;
            }

            return false;
        }

        public async Task<bool> CheckIfCanRepply(int accountId, string plantId, string commentId)
        {
            Plant? plant = await products.Find(p => p.Id.CompareTo(plantId) == 0)
                .FirstOrDefaultAsync();

            if(plant != null)
            {
                /* check only if have comments */
                if(plant.comments != null && plant.comments.Length > 0)
                {
                    Comment? comment = plant.comments
                        .FirstOrDefault(c => c.Id.CompareTo(commentId) == 0 && (c.isDeleted != null ? !c.isDeleted.Value : false));

                    if(comment != null)
                    {
                        if (comment.accountId == accountId) return false;
                        else
                        {
                            if(comment.replyTo != null)
                            {
                                /* does not allow to reply self */
                                Comment? post = plant.comments.FirstOrDefault(e => e.Id.CompareTo(comment.replyTo) == 0);
                                return post.accountId != accountId;
                            }

                            // are allowed
                            return true;
                        }
                    }
                }

                return false;
            }

            return false;
        }

        public async Task<PlantViewModel> GetProductAsync(string id)
        {
            Plant? product = await products.Find(p => p.Id == id).FirstOrDefaultAsync();

            if(product != null)
            {
                PlantViewModel plant = new PlantViewModel
                {
                    Id = product.Id,
                    name = product.name,
                    imageUrl = product.imageUrl,
                    description = product.description,
                    category = product.category
                };

                double rating = 0.0;

                if (product.comments != null)
                {
                    Comment[]? comments = product.comments.Where(c => c.isDeleted == null || (c.isDeleted != null && !c.isDeleted.Value))
                        .ToArray();

                    double sum = 0.0;
                    int n = comments.Length;

                    for (int j = 0; j < comments.Length; j++)
                    {
                        if (comments[j].rating.HasValue)
                            sum += comments[j].rating.Value;
                        else n--;
                    }

                    /* initial position index */
                    int[] index = new int[comments.Length];

                    /* visited nodes */
                    bool[] v = new bool[comments.Length];

                    for (int i = 0; i < comments.Length; i++)
                    {
                        index[i] = i;
                        v[i] = false;
                    }

                    /* sort post comments list by creation date ascending */
                    for (int i = 0; i < comments.Length - 1; i++)
                    {
                        for (int j = i + 1; j < comments.Length; j++)
                        {
                            if (comments[index[i]].createdAt.CompareTo(comments[index[j]].createdAt) > 0)
                            {
                                int t = index[i];
                                index[i] = index[j];
                                index[j] = t;
                            }
                        }
                    }

                    Queue<CommentViewModel> queue = new Queue<CommentViewModel>();
                    List<CommentViewModel> list = new List<CommentViewModel>();

                    /* forest of trees */
                    for (int i = 0; i < comments.Length; i++)
                    {
                        if (string.IsNullOrEmpty(comments[index[i]].replyTo))
                        {
                            Account? account = await bridgeContext.Account
                                .FirstOrDefaultAsync(u => u.Id == comments[index[i]].accountId);

                            CommentViewModel comment = new CommentViewModel
                            {
                                id = comments[index[i]].Id,
                                body = comments[index[i]].body,
                                createdAt = comments[index[i]].createdAt,
                                rating = comments[index[i]].rating,
                                accountId = comments[index[i]].accountId,
                                username = account.Username,
                                depth = 0
                            };

                            v[index[i]] = true;
                            queue.Enqueue(comment);
                        }
                    }

                    /* traverses the tree, using breadth-first search maner */
                    while (queue.Count > 0)
                    {
                        CommentViewModel parent = queue.Dequeue();
                        bool exists = false;

                        for (int i = 0; i < list.Count; i++)
                        {
                            if (list[i].id == parent.id)
                            {
                                exists = true;
                                break;
                            }
                        }

                        if (!exists) list.Add(parent);

                        for (int j = 0; j < comments.Length; j++)
                        {
                            if (parent.id == comments[index[j]].replyTo && !v[index[j]] && comments[index[j]].replyTo != null)
                            {
                                v[index[j]] = true;

                                Account? account = await bridgeContext.Account
                                    .FirstOrDefaultAsync(u => u.Id == comments[index[j]].accountId);

                                CommentViewModel comment = new CommentViewModel
                                {
                                    id = comments[index[j]].Id,
                                    body = comments[index[j]].body,
                                    createdAt = comments[index[j]].createdAt,
                                    rating = comments[index[j]].rating,
                                    accountId = comments[index[j]].accountId,
                                    username = account.Username,
                                    depth = parent.depth + 1
                                };

                                list.Add(comment);
                                queue.Enqueue(comment);
                            }
                        }
                    }

                    /* compute rating for presentation plant */
                    rating = sum / n;
                    plant.comments = list.ToArray();
                }

                plant.Stars = rating;
                return plant;
            }

            return null;
        }

        public async Task<Comment?> GetCommentAsync(string plantId, string commentId)
        {
            Plant? plant = await products.Find(p => p.Id == plantId)
                .FirstOrDefaultAsync();

            if(plant != null)
            {
                /* have comments */
                if(plant.comments != null && plant.comments.Length > 0)
                {
                    /* get valid comment */
                    Comment? comment = plant.comments
                        .FirstOrDefault(e => e.Id.CompareTo(commentId) == 0 && e.isDeleted == null || (e.isDeleted != null && !e.isDeleted.Value));

                    return comment;
                }
            }

            /* plant not exists */
            return null;
        }

        public async Task<int> RemoveCommentAsync(int accountId, string plantId, string commentId)
        {
            Plant? plant = await products.Find(p => p.Id == plantId)
                .FirstOrDefaultAsync();

            if(plant != null)
            {
                Comment? comment = plant.comments
                    .FirstOrDefault(c => c.Id.CompareTo(commentId) == 0 && c.accountId == accountId);

                if(comment != null)
                {
                    List<Comment> comments = new List<Comment>();
                    Queue<Comment> queue = new Queue<Comment>();
                    queue.Enqueue(comment);

                    if (plant.comments != null && plant.comments.Length > 0)
                        comments.AddRange(plant.comments);

                    /* remove child chat nodes recursively */
                    while (queue.Count > 0)
                    {
                        Comment node = queue.Dequeue();
                        node.isDeleted = true;

                        for(int j = 0; j < comments.Count; j++)
                        {
                            if (node.Id.CompareTo(comments[j].Id) == 0)
                                comments[j].isDeleted = true;
                        }

                        for(int k = 0; k < plant.comments.Length; k++)
                        {
                            if (plant.comments[k].replyTo != null && plant.comments[k].replyTo.CompareTo(node.Id) == 0)
                                queue.Enqueue(plant.comments[k]);
                        }
                    }
                    
                    /* remove successful */
                    plant.comments = comments.ToArray();
                    await products.ReplaceOneAsync(e => e.Id.CompareTo(plantId) == 0, plant);
                    return 1;
                }

                /* you have no rights */
                return 0;
            }

            /* plant not exists s*/
            return -1;
        }

        public async Task CreateProductAsync(Plant product) => await products.InsertOneAsync(product);

        public async Task CreateProductsAsync(Plant[] productList) => await products.InsertManyAsync(productList);

        public async Task RemoveProductAsync(string id) => await products.DeleteOneAsync(x => x.Id == id);

        public async Task RemoveProductsAsync() => await products.DeleteManyAsync(Builders<Plant>.Filter.Empty);
    }
}
