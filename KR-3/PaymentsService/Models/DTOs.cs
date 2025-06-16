using System.ComponentModel.DataAnnotations;

namespace PaymentsService.Models
{

    public class CreateAccountRequest
    {
        [Required]
        public Guid UserId { get; set; }
    }

    public class DepositRequest
    {
        [Required]
        public Guid UserId { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than 0")]
        public decimal Amount { get; set; }

        public string Description { get; set; } = string.Empty;
    }

    public class GetBalanceRequest
    {
        [Required]
        public Guid UserId { get; set; }
    }


    public class AccountResponse
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public decimal Balance { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class BalanceResponse
    {
        public Guid AccountId { get; set; }
        public Guid UserId { get; set; }
        public decimal Balance { get; set; }
    }

    public class TransactionResponse
    {
        public Guid Id { get; set; }
        public Guid AccountId { get; set; }
        public TransactionType Type { get; set; }
        public TransactionStatus Status { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
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