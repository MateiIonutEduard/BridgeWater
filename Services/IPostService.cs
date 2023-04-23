using BridgeWater.Models;

namespace BridgeWater.Services
{
    public interface IPostService
    {
        Task<PostRatingModel[]> GetPostsAsync(int id);
        Task<int> CreatePostAsync(PostRatingModel postRatingModel);
    }
}
