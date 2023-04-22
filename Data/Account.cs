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
        public string Avatar { get; set; }
        public bool IsAdmin { get; set; }
        [ForeignKey("AccountId")]
        public virtual ICollection<Order> Orders { get; set; }
        [ForeignKey("AccountId")]
        public virtual ICollection<Post> Posts { get; set; }
    }
}
