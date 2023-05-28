using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore;

namespace BridgeWater.Data
{
    public class BridgeContextFactory : IDesignTimeDbContextFactory<BridgeContext>
    {
        /* This fix is needed to Entity Framework, to create database migrations successfully. */
        public BridgeContext CreateDbContext(string[] args)
        {
            IConfigurationRoot configurationRoot = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json").Build();

            var builderContext = new DbContextOptionsBuilder<BridgeContext>();
            var connectionString = configurationRoot.GetConnectionString("PostgresBW");

            builderContext.UseNpgsql(connectionString);
            return new BridgeContext(builderContext.Options);
        }
    }
}
