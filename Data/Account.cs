#pragma warning disable

using System.ComponentModel.DataAnnotations.Schema;

namespace BridgeWater.Data
{
    public class Account
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Address { get; set; }
        public string Photo { get; set; }
        [ForeignKey("AccountId")]
        public virtual ICollection<Order> Orders { get; set; }
    }
}
