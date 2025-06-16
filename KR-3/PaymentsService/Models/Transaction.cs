using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PaymentsService.Models
{
    public enum TransactionType
    {
        Deposit,
        Withdrawal,
        PaymentAttempt
    }

    public enum TransactionStatus
    {
        Pending,
        Completed,
        Failed
    }

    public class Transaction
    {
        [Key]
        public Guid Id { get; set; }
        
        [Required]
        public Guid AccountId { get; set; }
        
        [ForeignKey("AccountId")]
        public Account? Account { get; set; }
        
        [Required]
        public TransactionType Type { get; set; }
        
        [Required]
        public TransactionStatus Status { get; set; }
        
        [Required]
        public decimal Amount { get; set; }
        
        public string Description { get; set; }

        public Guid? OrderId { get; set; }
        
        public DateTime CreatedAt { get; set; }
        
        public DateTime UpdatedAt { get; set; }
        
        public Transaction()
        {
            Id = Guid.NewGuid();
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
            Status = TransactionStatus.Pending;
            Description = string.Empty;
        }
    }
}