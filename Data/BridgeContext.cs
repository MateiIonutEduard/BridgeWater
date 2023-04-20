using Microsoft.EntityFrameworkCore;

namespace BridgeWater.Data
{
    public class BridgeContext : DbContext
    {
        public BridgeContext(DbContextOptions<BridgeContext> options) : base(options)
        { }

        public DbSet<Category> Category { get; set; }
        public DbSet<Product> Product { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Category>().ToTable("Category");
            modelBuilder.Entity<Product>().ToTable("Product");
        }
    }
}
