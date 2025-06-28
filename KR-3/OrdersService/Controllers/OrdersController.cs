using Microsoft.AspNetCore.Mvc;
using OrdersService.Models;
using OrdersService.Services;

namespace OrdersService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly ILogger<OrdersController> _logger;

        public OrdersController(
            IOrderService orderService,
            ILogger<OrdersController> logger)
        {
            _orderService = orderService;
            _logger = logger;
        }

        [HttpPost]
        [ProducesResponseType(typeof(OrderResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequest request)
        {
            try
            {
                var order = await _orderService.CreateOrderAsync(
                    request.UserId,
                    request.Amount,
                    request.Description);

                return CreatedAtAction(nameof(GetOrder), new { orderId = order.Id }, order);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create order for user {UserId}", request.UserId);
                return BadRequest(new ErrorResponse(ex.Message, StatusCodes.Status400BadRequest));
            }
        }

        [HttpGet("{orderId}")]
        [ProducesResponseType(typeof(OrderResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetOrder(Guid orderId)
        {
            try
            {
                var order = await _orderService.GetOrderAsync(orderId);
                return Ok(order);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Order {OrderId} not found", orderId);
                return NotFound(new ErrorResponse(ex.Message, StatusCodes.Status404NotFound));
            }
        }

        [HttpGet("user/{userId}")]
        [ProducesResponseType(typeof(OrderListResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetUserOrders(
            Guid userId,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            var orders = await _orderService.GetUserOrdersAsync(userId, page, pageSize);
            return Ok(orders);
        }
    }
}
