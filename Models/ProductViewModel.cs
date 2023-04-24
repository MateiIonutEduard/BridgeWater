using BridgeWater.Data;
#pragma warning disable

namespace BridgeWater.Models
{
    public class ProductViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int? CategoryId { get; set; }
        public string? Category { get; set; }
        public string TechInfo { get; set; }
        public double Stars { get; set; }
        public int Stock { get; set; }
        public double Price { get; set; }
    }
}
