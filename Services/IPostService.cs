using BridgeWater.Models;

namespace BridgeWater.Services
{
    public interface IPostService
    {
        Task<PostRatingModel[]> GetPostsAsync(int id);
        Task<int> CreatePostAsync(PostRatingModel postRatingModel);
        Task<bool> RemovePostAsync(int accountId, int postRatingId);
        Task<bool> HasPostRatingAsync(int accountId, int productId);
    }
}
