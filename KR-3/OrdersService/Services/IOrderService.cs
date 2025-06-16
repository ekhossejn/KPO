using OrdersService.Models;

namespace OrdersService.Services
{
    public interface IOrderService
    {
        Task<OrderResponse> CreateOrderAsync(Guid userId, decimal amount, string description);
        Task<OrderResponse> GetOrderAsync(Guid orderId);
        Task<OrderListResponse> GetUserOrdersAsync(Guid userId, int page = 1, int pageSize = 10);
        Task UpdateOrderStatusAsync(Guid orderId, OrderStatus status);
    }
}
