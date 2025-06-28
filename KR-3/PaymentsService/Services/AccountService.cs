using Microsoft.EntityFrameworkCore;
using PaymentsService.Data;
using PaymentsService.Models;
using Common.Models;

namespace PaymentsService.Services
{
    public class AccountService : IAccountService
    {
        private readonly PaymentsDbContext _dbContext;
        private readonly IDbContextFactory<PaymentsDbContext> _dbContextFactory;
        private readonly IMessageService _messageService;
        private readonly ILogger<AccountService> _logger;

        public AccountService(
            PaymentsDbContext dbContext,
            IDbContextFactory<PaymentsDbContext> dbContextFactory,
            IMessageService messageService,
            ILogger<AccountService> logger)
        {
            _dbContext = dbContext;
            _dbContextFactory = dbContextFactory;
            _messageService = messageService;
            _logger = logger;
        }

        public async Task<AccountResponse> CreateAccountAsync(Guid userId)
        {
            var existingAccount = await _dbContext.Accounts
                .FirstOrDefaultAsync(a => a.UserId == userId);

            if (existingAccount != null)
            {
                throw new InvalidOperationException($"Account for user {userId} already exists");
            }
            var account = new Account
            {
                UserId = userId,
                RowVersion = 1
            };

            _dbContext.Accounts.Add(account);
            await _dbContext.SaveChangesAsync();
            _logger.LogInformation("Created account {AccountId} for user {UserId}", account.Id, userId);
            return new AccountResponse
            {
                Id = account.Id,
                UserId = account.UserId,
                Balance = account.Balance,
                CreatedAt = account.CreatedAt
            };
        }

        public async Task<BalanceResponse> GetBalanceAsync(Guid userId)
        {
            var account = await GetAccountByUserIdAsync(userId);
            return new BalanceResponse
            {
                AccountId = account.Id,
                UserId = account.UserId,
                Balance = account.Balance
            };
        }

        public async Task<TransactionResponse> DepositAsync(Guid userId, decimal amount, string description)
        {
            if (amount <= 0)
            {
                throw new ArgumentException("Deposit amount must be greater than zero", nameof(amount));
            }

            var account = await GetAccountByUserIdAsync(userId);
            var transaction = new Transaction
            {
                AccountId = account.Id,
                Type = TransactionType.Deposit,
                Amount = amount,
                Description = description,
                Status = TransactionStatus.Completed
            };

            await UpdateAccountBalanceAsync(account, amount);

            _dbContext.Transactions.Add(transaction);
            await _dbContext.SaveChangesAsync();

            _logger.LogInformation("Deposited {Amount} to account {AccountId}", amount, account.Id);
            return new TransactionResponse
            {
                Id = transaction.Id,
                AccountId = transaction.AccountId,
                Type = transaction.Type,
                Status = transaction.Status,
                Amount = transaction.Amount,
                Description = transaction.Description,
                CreatedAt = transaction.CreatedAt
            };
        }

        public async Task<bool> ProcessPaymentAsync(Guid orderId, Guid userId, decimal amount, string description)
        {
            try
            {

                using var context = await _dbContextFactory.CreateDbContextAsync();
                var account = await context.Accounts
                    .FirstOrDefaultAsync(a => a.UserId == userId);

                if (account == null)
                {
                    throw new KeyNotFoundException($"Account not found for user {userId}");
                }
                if (account.Balance < amount)
                {
                    _logger.LogWarning("Insufficient balance for order {OrderId}. Required: {Amount}, Available: {Balance}",
                        orderId, amount, account.Balance);
                    var failedTransaction = new Transaction
                    {
                        AccountId = account.Id,
                        Type = TransactionType.PaymentAttempt,
                        Amount = amount,
                        Description = description,
                        OrderId = orderId,
                        Status = TransactionStatus.Failed
                    };

                    context.Transactions.Add(failedTransaction);
                    await context.SaveChangesAsync();
                    return false;
                }

                var transaction = new Transaction
                {
                    AccountId = account.Id,
                    Type = TransactionType.Withdrawal,
                    Amount = amount,
                    Description = description,
                    OrderId = orderId,
                    Status = TransactionStatus.Completed
                };

                account.Balance -= amount;
                account.UpdatedAt = DateTime.UtcNow;
                account.RowVersion++;
                context.Transactions.Add(transaction);
                await context.SaveChangesAsync();
                _logger.LogInformation("Payment of {Amount} processed for order {OrderId}", amount, orderId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing payment for order {OrderId}. Error details: {Message}, Stack trace: {StackTrace}",
                    orderId, ex.Message, ex.StackTrace);
                if (ex.Message.Contains("disposed context"))
                {
                    _logger.LogError("DbContext disposal issue detected. This may indicate a dependency injection or context lifetime issue.");
                }

                if (ex.InnerException != null)
                {
                    _logger.LogError("Inner exception: {InnerMessage}, Stack trace: {InnerStackTrace}",
                        ex.InnerException.Message, ex.InnerException.StackTrace);
                }
                return false;
            }
        }

        private async Task<Account> GetAccountByUserIdAsync(Guid userId)
        {
            var account = await _dbContext.Accounts
                .FirstOrDefaultAsync(a => a.UserId == userId);

            if (account == null)
            {
                throw new KeyNotFoundException($"Account not found for user {userId}");
            }
            return account;
        }

        private async Task UpdateAccountBalanceAsync(Account account, decimal amountChange)
        {
            var success = false;
            var retryCount = 0;
            const int maxRetries = 3;

            while (!success && retryCount < maxRetries)
            {
                try
                {
                    if (retryCount > 0)
                    {
                        await _dbContext.Entry(account).ReloadAsync();
                    }
                    account.Balance += amountChange;
                    account.UpdatedAt = DateTime.UtcNow;
                    account.RowVersion++;

                    await _dbContext.SaveChangesAsync();
                    success = true;
                }
                catch (DbUpdateConcurrencyException)
                {
                    retryCount++;
                    if (retryCount >= maxRetries)
                    {
                        throw new InvalidOperationException("Failed to update account balance due to concurrent modifications");
                    }
                }
            }
        }
    }
}
