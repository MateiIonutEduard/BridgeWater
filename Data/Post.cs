#pragma warning disable

namespace BridgeWater.Data
{
    public class Post
    {
        public int Id { get; set; }
        public string? Body { get; set; }
        public int? Rating { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool? IsDeleted { get; set; }
        public int? ReplyTo { get; set; }
        public int AccountId { get; set; }
        public Account Account { get; set; }
        public int ProductId { get; set; }
        public Product Product { get; set; }
    }
}
