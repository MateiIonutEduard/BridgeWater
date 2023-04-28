namespace BridgeWater.Models
{
    public class ProductOrderViewList
    {
        public int pages { get; set; }
        public int results { get; set; }
        public OrderViewModel[]? orderViewModels { get; set; }
    }
}
