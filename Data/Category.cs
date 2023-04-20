#pragma warning disable

using System.ComponentModel.DataAnnotations.Schema;

namespace BridgeWater.Data
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }
        [ForeignKey("CategoryId")]
        public virtual ICollection<Product> Products { get; set; }
    }
}
