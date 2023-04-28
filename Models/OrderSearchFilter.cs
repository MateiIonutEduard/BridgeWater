#pragma warning disable

namespace BridgeWater.Models
{
    public class OrderSearchFilter
    {
        public string? ProductName { get; set; }
        public int? CategoryId { get; set; }
        public bool? IsCanceled { get; set; }
    }
}
