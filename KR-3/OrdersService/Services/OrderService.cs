using Microsoft.EntityFrameworkCore;
using OrdersService.Data;
using OrdersService.Models;
using Common.Models;

namespace OrdersService.Services
{
    public class OrderService : IOrderService
    {
        private readonly OrdersDbContext _dbContext;
        private readonly IMessageService _messageService;
        private readonly ILogger<OrderService> _logger;

        public OrderService(
            OrdersDbContext dbContext,
            IMessageService messageService,
            ILogger<OrderService> logger)
        {
            _dbContext = dbContext;
            _messageService = messageService;
            _logger = logger;
        }

        public async Task<OrderResponse> CreateOrderAsync(Guid userId, decimal amount, string description)
        {
            using var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                var order = new Order
                {
                    UserId = userId,
                    Amount = amount,
                    Description = description,
                    Status = OrderStatus.New
                };

                _dbContext.Orders.Add(order);
                await _dbContext.SaveChangesAsync();
                var paymentRequest = new OrderPaymentRequestMessage
                {
                    OrderId = order.Id,
                    UserId = userId,
                    Amount = amount,
                    Description = description
                };
                await _messageService.SaveOutboxMessageAsync(paymentRequest, nameof(OrderPaymentRequestMessage));
                await transaction.CommitAsync();

                _logger.LogInformation("Created order {OrderId} for user {UserId}", order.Id, userId);
                return MapToOrderResponse(order);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error creating order for user {UserId}", userId);
                throw;
            }
        }

        public async Task<OrderResponse> GetOrderAsync(Guid orderId)
        {
            var order = await _dbContext.Orders
                .FirstOrDefaultAsync(o => o.Id == orderId);
            if (order == null)
            {
                throw new KeyNotFoundException($"Order {orderId} not found");
            }
            return MapToOrderResponse(order);
        }

        public async Task<OrderListResponse> GetUserOrdersAsync(Guid userId, int page = 1, int pageSize = 10)
        {
            var query = _dbContext.Orders
                .Where(o => o.UserId == userId)
                .OrderByDescending(o => o.CreatedAt);

            var totalCount = await query.CountAsync();
            var orders = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
            return new OrderListResponse
            {
                Orders = orders.Select(MapToOrderResponse).ToList(),
                TotalCount = totalCount
            };
        }

        public async Task UpdateOrderStatusAsync(Guid orderId, OrderStatus status)
        {
            var order = await _dbContext.Orders
                .FirstOrDefaultAsync(o => o.Id == orderId);
            if (order == null)
            {
                throw new KeyNotFoundException($"Order {orderId} not found");
            }
            order.Status = status;
            order.UpdatedAt = DateTime.UtcNow;
            await _dbContext.SaveChangesAsync();

            _logger.LogInformation("Updated order {OrderId} status to {Status}", orderId, status);
        }

        private OrderResponse MapToOrderResponse(Order order)
        {
            return new OrderResponse
            {
                Id = order.Id,
                UserId = order.UserId,
                Amount = order.Amount,
                Description = order.Description,
                Status = order.Status.ToString(),
                CreatedAt = order.CreatedAt
            };
        }
    }
}
