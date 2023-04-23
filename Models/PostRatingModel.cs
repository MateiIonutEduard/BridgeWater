using BridgeWater.Data;

namespace BridgeWater.Models
{
    public class PostRatingModel
    {
        public int? id { get; set; }
        public string? username { get; set; }
        public string? body { get; set; }
        public int? rating { get; set; }
        public DateTime? createdAt { get; set; }
        public bool? isDeleted { get; set; }
        public int accountId { get; set; }
        public int productId { get; set; }
    }
}
