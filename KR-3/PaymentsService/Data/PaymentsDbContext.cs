using Microsoft.EntityFrameworkCore;
using PaymentsService.Models;

namespace PaymentsService.Data
{
    public class PaymentsDbContext : DbContext
    {
        public DbSet<Account> Accounts { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<OutboxMessage> OutboxMessages { get; set; }
        public DbSet<InboxMessage> InboxMessages { get; set; }

        public PaymentsDbContext(DbContextOptions<PaymentsDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Account>()
                .HasIndex(a => a.UserId)
                .IsUnique();
            modelBuilder.Entity<Account>()
                .Property(a => a.Balance)
                .HasPrecision(18, 2);
            modelBuilder.Entity<Account>()
                .Property(a => a.RowVersion)
                .IsConcurrencyToken()
                .HasDefaultValue(1L);
            modelBuilder.Entity<Transaction>()
                .Property(t => t.Amount)
                .HasPrecision(18, 2);
            modelBuilder.Entity<Transaction>()
                .HasOne(t => t.Account)
                .WithMany(a => a.Transactions)
                .HasForeignKey(t => t.AccountId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<OutboxMessage>()
                .HasIndex(m => m.Status);
            modelBuilder.Entity<InboxMessage>()
                .HasIndex(m => m.MessageId)
                .IsUnique();
            modelBuilder.Entity<InboxMessage>()
                .HasIndex(m => m.Status);
        }
    }
}
