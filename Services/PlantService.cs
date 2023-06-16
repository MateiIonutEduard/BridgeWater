﻿using BridgeWater.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System.Net;
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

                Comment[]? comments = product.comments;
                List<CommentViewModel> commentList = new List<CommentViewModel>();
                double rating = 0.0;

                if(comments != null)
                {
                    double sum = comments.Where(c => c.rating != null && c.isDeleted == null || (c.isDeleted != null && !c.isDeleted.Value))
                        .Sum(c => c.rating.Value);

                    /* copy comments into layer objects */
                    for(int i = 0; i < comments.Length; i++)
                    {
                        if (comments[i].isDeleted != null && comments[i].isDeleted.Value)
                            continue;

                        CommentViewModel comment = new CommentViewModel
                        {
                            id = comments[i].Id,
                            body = comments[i].body,
                            createdAt = comments[i].createdAt,
                            depth = 0,
                            rating = comments[i].rating,
                            accountId = comments[i].accountId
                        };

                        /* add new one */
                        commentList.Add(comment);
                    }

                    /* compute rating for presentation plant */
                    rating = sum / comments.Count();
                    plant.comments = commentList.ToArray();
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
                    
                    for(int k = 0; k < plant.comments.Length; k++)
                    {
                        /* ignore selected comment */
                        if (plant.comments[k].Id.CompareTo(commentId) == 0) comment.isDeleted = true;
                        comments.Add(plant.comments[k]);
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
