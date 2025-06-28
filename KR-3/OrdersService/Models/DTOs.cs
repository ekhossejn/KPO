using System.ComponentModel.DataAnnotations;

namespace OrdersService.Models
{
    public class CreateOrderRequest
    {
        [Required]
        public Guid UserId { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than 0")]
        public decimal Amount { get; set; }

        [Required]
        public string Description { get; set; } = string.Empty;
    }


    public class OrderResponse
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }

    public class OrderListResponse
    {
        public List<OrderResponse> Orders { get; set; } = new List<OrderResponse>();
        public int TotalCount { get; set; }
    }

    public class ErrorResponse
    {
        public string Message { get; set; }
        public int StatusCode { get; set; }

        public ErrorResponse(string message, int statusCode)
        {
            Message = message;
            StatusCode = statusCode;
        }
    }
}
