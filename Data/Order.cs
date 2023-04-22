#pragma warning disable
using System.ComponentModel.DataAnnotations.Schema;

namespace BridgeWater.Data
{
    public class Order
    {
        public int Id { get; set; }
        public int AccountId { get; set; }
        public Account Account { get; set; }
        public int ProductOrderId { get; set; }
        public Product Product { get; set; }
        public int Stock { get; set; }
        public bool? IsCanceled { get; set; }
    }
}
