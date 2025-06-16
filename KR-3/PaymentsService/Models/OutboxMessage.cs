using System.ComponentModel.DataAnnotations;

namespace PaymentsService.Models
{
    public enum MessageStatus
    {
        Pending,
        Processed,
        Failed
    }

    public class OutboxMessage
    {
        [Key]
        public Guid Id { get; set; }
        
        [Required]
        public string MessageType { get; set; }
        
        [Required]
        public string MessageContent { get; set; }
        
        [Required]
        public MessageStatus Status { get; set; }
        
        public int RetryCount { get; set; }
        
        public string? Error { get; set; }
        
        public DateTime CreatedAt { get; set; }
        
        public DateTime? ProcessedAt { get; set; }
        
        public OutboxMessage()
        {
            Id = Guid.NewGuid();
            CreatedAt = DateTime.UtcNow;
            Status = MessageStatus.Pending;
            RetryCount = 0;
            MessageType = string.Empty;
            MessageContent = string.Empty;
        }
    }
}