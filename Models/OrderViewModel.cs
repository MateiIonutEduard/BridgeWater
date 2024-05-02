namespace BridgeWater.Models
{
    public class OrderViewModel
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string? ProductName { get; set; }
        public double Price { get; set; }
        public bool IsCanceled { get; set; }
        public bool IsPayed { get; set; }
        public int Stock { get; set; }
    }
}
