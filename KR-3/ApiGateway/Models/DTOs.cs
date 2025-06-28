using System.ComponentModel.DataAnnotations;

namespace ApiGateway.Models
{
    /// <summary>
    /// Request to create a new account
    /// </summary>
    public class CreateAccountRequest
    {
        /// <summary>
        /// The unique identifier of the user
        /// </summary>
        [Required]
        public Guid UserId { get; set; }
    }

    /// <summary>
    /// Response from account creation or retrieval
    /// </summary>
    public class AccountResponse
    {
        /// <summary>
        /// The unique identifier of the account
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// The unique identifier of the user who owns the account
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// The current balance of the account
        /// </summary>
        public decimal Balance { get; set; }

        /// <summary>
        /// When the account was created
        /// </summary>
        public DateTime CreatedAt { get; set; }
    }

    /// <summary>
    /// Response containing account balance information
    /// </summary>
    public class BalanceResponse
    {
        /// <summary>
        /// The unique identifier of the account
        /// </summary>
        public Guid AccountId { get; set; }

        /// <summary>
        /// The unique identifier of the user who owns the account
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// The current balance of the account
        /// </summary>
        public decimal Balance { get; set; }
    }

    /// <summary>
    /// Request to deposit funds into an account
    /// </summary>
    public class DepositRequest
    {
        /// <summary>
        /// The unique identifier of the user who owns the account
        /// </summary>
        [Required]
        public Guid UserId { get; set; }

        /// <summary>
        /// The amount to deposit
        /// </summary>
        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be positive")]
        public decimal Amount { get; set; }

        /// <summary>
        /// Description of the deposit
        /// </summary>
        public string Description { get; set; } = string.Empty;
    }

    /// <summary>
    /// Request to create a new order
    /// </summary>
    public class CreateOrderRequest
    {
        /// <summary>
        /// The unique identifier of the user
        /// </summary>
        [Required]
        public Guid UserId { get; set; }

        /// <summary>
        /// The amount of the order
        /// </summary>
        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be positive")]
        public decimal Amount { get; set; }

        /// <summary>
        /// Description of the order
        /// </summary>
        [Required]
        public string Description { get; set; } = string.Empty;
    }

    /// <summary>
    /// Response containing order details
    /// </summary>
    public class OrderResponse
    {
        /// <summary>
        /// The unique identifier of the order
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// The unique identifier of the user who placed the order
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// The amount of the order
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// Description of the order
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Current status of the order
        /// </summary>
        public string Status { get; set; } = string.Empty;

        /// <summary>
        /// When the order was created
        /// </summary>
        public DateTime CreatedAt { get; set; }
    }

    /// <summary>
    /// Response containing a list of orders
    /// </summary>
    public class OrderListResponse
    {
        /// <summary>
        /// List of orders
        /// </summary>
        public List<OrderResponse> Orders { get; set; } = new List<OrderResponse>();

        /// <summary>
        /// Total number of orders
        /// </summary>
        public int TotalCount { get; set; }
    }

    /// <summary>
    /// Error response
    /// </summary>
    public class ErrorResponse
    {
        /// <summary>
        /// Error message
        /// </summary>
        public string Message { get; set; } = string.Empty;

        /// <summary>
        /// HTTP status code
        /// </summary>
        public int StatusCode { get; set; }

        /// <summary>
        /// Create a new error response
        /// </summary>
        public ErrorResponse(string message, int statusCode)
        {
            Message = message;
            StatusCode = statusCode;
        }
    }
}
