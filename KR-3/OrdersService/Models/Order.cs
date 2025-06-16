using System.ComponentModel.DataAnnotations;

namespace OrdersService.Models
{
    public enum OrderStatus
    {
        New,
        Finished,
        Cancelled
    }

    public class Order
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public Guid UserId { get; set; }

        [Required]
        public decimal Amount { get; set; }

        [Required]
        public string Description { get; set; } = string.Empty;

        [Required]
        public OrderStatus Status { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        public Order()
        {
            Id = Guid.NewGuid();
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
            Status = OrderStatus.New;
            Description = string.Empty;
        }
    }
}
