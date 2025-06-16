using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;


namespace OrdersService.Data
{
    public class OrdersDbContextFactory : IDesignTimeDbContextFactory<OrdersDbContext>
    {
        public OrdersDbContext CreateDbContext(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false)
                .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development"}.json", optional: true)
                .Build();
            var optionsBuilder = new DbContextOptionsBuilder<OrdersDbContext>();
            var connectionString = configuration.GetConnectionString("OrdersDb");
            optionsBuilder.UseNpgsql(connectionString);
            return new OrdersDbContext(optionsBuilder.Options);
        }
    }
}
