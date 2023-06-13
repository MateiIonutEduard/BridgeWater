using BridgeWater.Data;
using BridgeWater.Models;

namespace BridgeWater.Services
{
    public interface IPostService
    {
        Task<Post?> GetPostAsync(int id);
        Task<PostRatingModel[]> GetPostsAsync(int id);
        Task<bool> CheckIfCanRepply(string userId, int postId);
        Task<int> CreatePostAsync(PostRatingModel postRatingModel);
        Task<int> CreateReplyPostAsync(PostRatingModel postRatingModel);
        Task<bool> RemovePostAsync(int accountId, int postRatingId);
        Task<bool> HasPostRatingAsync(int accountId, int productId);
    }
}
