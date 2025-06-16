using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using PaymentsService.Data;
using PaymentsService.Models;
using PaymentsService.Services;
using Xunit;

namespace PaymentsService.Tests
{
    public class AccountServiceTests
    {
        private readonly DbContextOptions<PaymentsDbContext> _dbContextOptions;
        private readonly Mock<IMessageService> _mockMessageService;
        private readonly Mock<ILogger<AccountService>> _mockLogger;
        private readonly Mock<IDbContextFactory<PaymentsDbContext>> _mockDbContextFactory;

        public AccountServiceTests()
        {
            _dbContextOptions = new DbContextOptionsBuilder<PaymentsDbContext>()
                .UseInMemoryDatabase(databaseName: "TestPaymentsDb_" + Guid.NewGuid().ToString())
                .Options;

            _mockMessageService = new Mock<IMessageService>();
            _mockLogger = new Mock<ILogger<AccountService>>();
            _mockDbContextFactory = new Mock<IDbContextFactory<PaymentsDbContext>>();
        }

        private AccountService CreateAccountService(PaymentsDbContext dbContext)
        {
            _mockDbContextFactory
                .Setup(f => f.CreateDbContextAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(dbContext);

            return new AccountService(
                dbContext,
                _mockDbContextFactory.Object,
                _mockMessageService.Object,
                _mockLogger.Object);
        }

        [Fact]
        public async Task CreateAccount_ShouldCreateNewAccount()
        {
            using var dbContext = new PaymentsDbContext(_dbContextOptions);
            var accountService = CreateAccountService(dbContext);
            var userId = Guid.NewGuid();
            var result = await accountService.CreateAccountAsync(userId);
            Assert.NotNull(result);
            Assert.Equal(userId, result.UserId);
            Assert.Equal(0, result.Balance);

            var savedAccount = await dbContext.Accounts.FirstOrDefaultAsync(a => a.UserId == userId);
            Assert.NotNull(savedAccount);
            Assert.Equal(userId, savedAccount.UserId);
            Assert.Equal(0, savedAccount.Balance);
        }

        [Fact]
        public async Task CreateAccount_ShouldThrowException_WhenAccountAlreadyExists()
        {
            using var dbContext = new PaymentsDbContext(_dbContextOptions);
            var accountService = CreateAccountService(dbContext);
            var userId = Guid.NewGuid();
            await accountService.CreateAccountAsync(userId);
            await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            {
                await accountService.CreateAccountAsync(userId);
            });
        }

        [Fact]
        public async Task GetBalance_ShouldReturnCorrectBalance()
        {
            using var dbContext = new PaymentsDbContext(_dbContextOptions);
            var accountService = CreateAccountService(dbContext);
            var userId = Guid.NewGuid();
            await accountService.CreateAccountAsync(userId);

            var result = await accountService.GetBalanceAsync(userId);
            Assert.NotNull(result);
            Assert.Equal(userId, result.UserId);
            Assert.Equal(0, result.Balance);
        }

        [Fact]
        public async Task GetBalance_ShouldThrowException_WhenAccountNotFound()
        {
            using var dbContext = new PaymentsDbContext(_dbContextOptions);
            var accountService = CreateAccountService(dbContext);
            var userId = Guid.NewGuid();
            await Assert.ThrowsAsync<KeyNotFoundException>(async () =>
            {
                await accountService.GetBalanceAsync(userId);
            });
        }

        [Fact]
        public async Task Deposit_ShouldIncreaseBalance()
        {
            using var dbContext = new PaymentsDbContext(_dbContextOptions);
            var accountService = CreateAccountService(dbContext);
            var userId = Guid.NewGuid();
            var depositAmount = 100m;
            await accountService.CreateAccountAsync(userId);

            var result = await accountService.DepositAsync(userId, depositAmount, "Test deposit");
            Assert.NotNull(result);
            Assert.Equal(depositAmount, result.Amount);
            Assert.Equal(TransactionType.Deposit, result.Type);
            Assert.Equal(TransactionStatus.Completed, result.Status);

            var balance = await accountService.GetBalanceAsync(userId);
            Assert.Equal(depositAmount, balance.Balance);
        }

        [Fact]
        public async Task ProcessPayment_ShouldReturnTrue_WhenSufficientFunds()
        {
            var dbContext = new PaymentsDbContext(_dbContextOptions);
            var accountService = CreateAccountService(dbContext);
            var userId = Guid.NewGuid();
            var orderId = Guid.NewGuid();
            var depositAmount = 100m;
            var paymentAmount = 50m;

            await accountService.CreateAccountAsync(userId);
            await accountService.DepositAsync(userId, depositAmount, "Initial deposit");

            var result = await accountService.ProcessPaymentAsync(orderId, userId, paymentAmount, "Test payment");
            Assert.True(result);
            using var verificationContext = new PaymentsDbContext(_dbContextOptions);
            var account = await verificationContext.Accounts.FirstOrDefaultAsync(a => a.UserId == userId);
            Assert.NotNull(account);
            Assert.Equal(depositAmount - paymentAmount, account.Balance);
        }

        [Fact]
        public async Task ProcessPayment_ShouldReturnFalse_WhenInsufficientFunds()
        {
            var dbContext = new PaymentsDbContext(_dbContextOptions);
            var accountService = CreateAccountService(dbContext);
            var userId = Guid.NewGuid();
            var orderId = Guid.NewGuid();
            var depositAmount = 30m;
            var paymentAmount = 50m;

            await accountService.CreateAccountAsync(userId);
            await accountService.DepositAsync(userId, depositAmount, "Initial deposit");
            var result = await accountService.ProcessPaymentAsync(orderId, userId, paymentAmount, "Test payment");
            Assert.False(result);
            using var verificationContext = new PaymentsDbContext(_dbContextOptions);
            var account = await verificationContext.Accounts.FirstOrDefaultAsync(a => a.UserId == userId);
            Assert.NotNull(account);
            Assert.Equal(depositAmount, account.Balance);
        }
    }
}
