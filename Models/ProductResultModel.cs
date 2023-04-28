namespace BridgeWater.Models
{
    public class ProductResultModel
    {
        public int Pages { get; set; }
        public int Results { get; set; }
        public ProductViewModel[]? ProductViewModels { get; set; }
    }
}
