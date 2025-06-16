using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using OrdersService.Data;
using OrdersService.Models;
using OrdersService.Services;
using Common.Models;
using Xunit;

namespace OrdersService.Tests
{
    public class OrderServiceTests
    {
        private readonly DbContextOptions<OrdersDbContext> _dbContextOptions;
        private readonly Mock<IMessageService> _mockMessageService;
        private readonly Mock<ILogger<OrderService>> _mockLogger;

        public OrderServiceTests()
        {
            _dbContextOptions = new DbContextOptionsBuilder<OrdersDbContext>()
                .UseInMemoryDatabase(databaseName: "TestOrdersDb_" + Guid.NewGuid().ToString())
                .ConfigureWarnings(w => w.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.InMemoryEventId.TransactionIgnoredWarning))
                .Options;

            _mockMessageService = new Mock<IMessageService>();
            _mockLogger = new Mock<ILogger<OrderService>>();
        }

        [Fact]
        public async Task CreateOrder_ShouldCreateNewOrder()
        {
            using var dbContext = new OrdersDbContext(_dbContextOptions);
            var orderService = new OrderService(dbContext, _mockMessageService.Object, _mockLogger.Object);
            var userId = Guid.NewGuid();
            var amount = 100m;
            var description = "Test order";
            _mockMessageService.Setup(m => m.SaveOutboxMessageAsync(
                It.IsAny<OrderPaymentRequestMessage>(),
                It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            var result = await orderService.CreateOrderAsync(userId, amount, description);

            Assert.NotNull(result);
            Assert.Equal(userId, result.UserId);
            Assert.Equal(amount, result.Amount);
            Assert.Equal(description, result.Description);
            Assert.Equal(OrderStatus.New.ToString(), result.Status);

            var savedOrder = await dbContext.Orders.FirstOrDefaultAsync(o => o.Id == Guid.Parse(result.Id.ToString()));
            Assert.NotNull(savedOrder);
            Assert.Equal(userId, savedOrder.UserId);
            Assert.Equal(amount, savedOrder.Amount);
            Assert.Equal(description, savedOrder.Description);
            Assert.Equal(OrderStatus.New, savedOrder.Status);

            _mockMessageService.Verify(m => m.SaveOutboxMessageAsync(
                It.Is<OrderPaymentRequestMessage>(msg =>
                    msg.UserId == userId &&
                    msg.Amount == amount &&
                    msg.Description == description),
                nameof(OrderPaymentRequestMessage)),
                Times.Once);
        }

        [Fact]
        public async Task GetOrder_ShouldReturnOrder_WhenOrderExists()
        {
            using var dbContext = new OrdersDbContext(_dbContextOptions);
            var orderService = new OrderService(dbContext, _mockMessageService.Object, _mockLogger.Object);
            var userId = Guid.NewGuid();

            var createdOrder = await orderService.CreateOrderAsync(userId, 100m, "Test order");
            var orderId = Guid.Parse(createdOrder.Id.ToString());

            var result = await orderService.GetOrderAsync(orderId);

            Assert.NotNull(result);
            Assert.Equal(orderId, Guid.Parse(result.Id.ToString()));
            Assert.Equal(userId, result.UserId);
            Assert.Equal(100m, result.Amount);
            Assert.Equal("Test order", result.Description);
            Assert.Equal(OrderStatus.New.ToString(), result.Status);
        }

        [Fact]
        public async Task GetOrder_ShouldThrowException_WhenOrderNotFound()
        {
            using var dbContext = new OrdersDbContext(_dbContextOptions);
            var orderService = new OrderService(dbContext, _mockMessageService.Object, _mockLogger.Object);
            var orderId = Guid.NewGuid();

            await Assert.ThrowsAsync<KeyNotFoundException>(async () =>
            {
                await orderService.GetOrderAsync(orderId);
            });
        }

        [Fact]
        public async Task GetUserOrders_ShouldReturnUserOrders()
        {
            using var dbContext = new OrdersDbContext(_dbContextOptions);
            var orderService = new OrderService(dbContext, _mockMessageService.Object, _mockLogger.Object);
            var userId = Guid.NewGuid();
            var otherUserId = Guid.NewGuid();

            await orderService.CreateOrderAsync(userId, 100m, "Order 1");
            await orderService.CreateOrderAsync(userId, 200m, "Order 2");
            await orderService.CreateOrderAsync(otherUserId, 300m, "Other user order");

            var result = await orderService.GetUserOrdersAsync(userId);

            Assert.NotNull(result);
            Assert.Equal(2, result.Orders.Count);
            Assert.Equal(2, result.TotalCount);
            Assert.All(result.Orders, order => Assert.Equal(userId, order.UserId));
        }

        [Fact]
        public async Task UpdateOrderStatus_ShouldUpdateStatus()
        {

            using var dbContext = new OrdersDbContext(_dbContextOptions);
            var orderService = new OrderService(dbContext, _mockMessageService.Object, _mockLogger.Object);
            var userId = Guid.NewGuid();
            var createdOrder = await orderService.CreateOrderAsync(userId, 100m, "Test order");
            var orderId = Guid.Parse(createdOrder.Id.ToString());
            await orderService.UpdateOrderStatusAsync(orderId, OrderStatus.Finished);

            var updatedOrder = await orderService.GetOrderAsync(orderId);
            Assert.Equal(OrderStatus.Finished.ToString(), updatedOrder.Status);
        }

        [Fact]
        public async Task UpdateOrderStatus_ShouldThrowException_WhenOrderNotFound()
        {

            using var dbContext = new OrdersDbContext(_dbContextOptions);
            var orderService = new OrderService(dbContext, _mockMessageService.Object, _mockLogger.Object);
            var orderId = Guid.NewGuid();

            await Assert.ThrowsAsync<KeyNotFoundException>(async () =>
            {
                await orderService.UpdateOrderStatusAsync(orderId, OrderStatus.Finished);
            });
        }
    }
}
