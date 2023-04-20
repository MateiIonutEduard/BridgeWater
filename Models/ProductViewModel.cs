using BridgeWater.Data;
#pragma warning disable

namespace BridgeWater.Models
{
    public class ProductViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string TechInfo { get; set; }
        public int Stock { get; set; }
        public double Price { get; set; }
    }
}
