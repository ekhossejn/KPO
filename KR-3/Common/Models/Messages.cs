namespace Common.Models
{
    public class OrderPaymentRequestMessage
    {
        public Guid OrderId { get; set; }
        public Guid UserId { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; } = string.Empty;
    }


    public class PaymentResultMessage
    {
        public Guid OrderId { get; set; }
        public bool IsSuccessful { get; set; }
        public string FailureReason { get; set; } = string.Empty;
    }
}
