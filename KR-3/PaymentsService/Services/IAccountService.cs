using PaymentsService.Models;

namespace PaymentsService.Services
{
    public interface IAccountService
    {
        Task<AccountResponse> CreateAccountAsync(Guid userId);
        Task<BalanceResponse> GetBalanceAsync(Guid userId);
        Task<TransactionResponse> DepositAsync(Guid userId, decimal amount, string description);
        Task<bool> ProcessPaymentAsync(Guid orderId, Guid userId, decimal amount, string description);
    }
}