using BridgeWater.Data;
using BridgeWater.Models;
using Microsoft.EntityFrameworkCore;

namespace BridgeWater.Services
{
    public class PostService : IPostService
    {
        readonly BridgeContext bridgeContext;
        public PostService(BridgeContext bridgeContext)
        { this.bridgeContext = bridgeContext; }

        public async Task<int> CreatePostAsync(PostRatingModel postRatingModel)
        {
            Post? post = await bridgeContext.Post
                .FirstOrDefaultAsync(e => Exists(e, postRatingModel));

            if (post == null)
            {
                if(!string.IsNullOrEmpty(postRatingModel.body) || postRatingModel.rating != null)
                {
                    post = new Post
                    {
                        Body = postRatingModel.body,
                        Rating = postRatingModel.rating,
                        AccountId = postRatingModel.accountId,
                        ProductId = postRatingModel.productId,
                        CreatedAt = DateTime.UtcNow,
                        IsDeleted = false
                    };

                    // save account post
                    bridgeContext.Post.Add(post);
                    await bridgeContext.SaveChangesAsync();
                    return 1;
                }

                // invalid post
                return 0;
            }

            // user post exists
            return -1;
        }

        public async Task<PostRatingModel[]> GetPostsAsync(int id)
        {
            // fetch only posts that are not removed
            PostRatingModel[] postRatingModels = (from p in await bridgeContext.Post.ToListAsync()
             join a in await bridgeContext.Account.ToListAsync()
             on p.AccountId equals a.Id where p.IsDeleted == null || (p.IsDeleted != null && !p.IsDeleted.Value)
             select new PostRatingModel
             {
                 id = p.Id,
                 body = p.Body,
                 rating = p.Rating,
                 accountId = p.AccountId,
                 isDeleted = p.IsDeleted,
                 createdAt = p.CreatedAt,
                 username = a.Username
             }).ToArray();

            return postRatingModels;
        }

        private bool Exists(Post post, PostRatingModel postRatingModel)
        {
            bool isFeed = post.AccountId == postRatingModel.accountId && post.ProductId == postRatingModel.productId;
            bool isDeleted = post.IsDeleted != null ? post.IsDeleted.Value : false;
            return isFeed && isDeleted;
        }
    }
}
