using System.ComponentModel.DataAnnotations;

namespace PaymentsService.Models
{
    public class Account
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public Guid UserId { get; set; }

        [Required]
        public decimal Balance { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        public long RowVersion { get; set; }

        public ICollection<Transaction> Transactions { get; set; }

        public Account()
        {
            Id = Guid.NewGuid();
            Balance = 0;
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
            Transactions = new List<Transaction>();
            RowVersion = 1;
        }
    }
}
