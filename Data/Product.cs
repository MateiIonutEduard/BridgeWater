#pragma warning disable

namespace BridgeWater.Data
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string LogoImage { get; set; }
        public string PosterImage { get; set; } 
        public string TechInfo { get; set; }
        public int Stock { get; set; }
        public double Price { get; set; }
    }
}
