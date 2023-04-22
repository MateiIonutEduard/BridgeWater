using Microsoft.EntityFrameworkCore;

namespace BridgeWater.Data
{
    public class BridgeContext : DbContext
    {
        public BridgeContext(DbContextOptions<BridgeContext> options) : base(options)
        { }

        public DbSet<Account> Account { get; set; }
        public DbSet<Post> Post { get; set; }
        public DbSet<Order> Order { get; set; }
        public DbSet<Category> Category { get; set; }
        public DbSet<Product> Product { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Account>().ToTable("Account");
            modelBuilder.Entity<Post>().ToTable("Post");
            modelBuilder.Entity<Order>().ToTable("Order");
            modelBuilder.Entity<Category>().ToTable("Category");
            modelBuilder.Entity<Product>().ToTable("Product");
        }
    }
}
