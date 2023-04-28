#pragma warning disable

namespace BridgeWater.Models
{
    public class ProductModel
    {
        public string name { get; set; }
        public string description { get; set; }
        public IFormFile logo { get; set; }
        public IFormFile poster { get; set; }
        public string techInfo { get; set; }

        public int categoryId { get; set; }
        public int stock { get; set; }
        public double price { get; set; }
    }
}
