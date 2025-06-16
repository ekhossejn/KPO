using Microsoft.EntityFrameworkCore;
using OrdersService.Models;

namespace OrdersService.Data
{
    public class OrdersDbContext : DbContext
    {
        public DbSet<Order> Orders { get; set; }
        public DbSet<OutboxMessage> OutboxMessages { get; set; }

        public OrdersDbContext(DbContextOptions<OrdersDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Order>()
                .Property(o => o.Amount)
                .HasPrecision(18, 2);
            modelBuilder.Entity<Order>()
                .HasIndex(o => o.UserId);
            modelBuilder.Entity<OutboxMessage>()
                .HasIndex(m => m.Status);
        }
    }
}
