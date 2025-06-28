using System.ComponentModel.DataAnnotations;

namespace PaymentsService.Models
{
    public class InboxMessage
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
        

        [Required]
        public Guid MessageId { get; set; }
        
        public DateTime CreatedAt { get; set; }
        
        public DateTime? ProcessedAt { get; set; }
        
        public InboxMessage()
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