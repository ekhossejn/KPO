using System.Text;
using System.Text.Json;
using ApiGateway.Models;
using Microsoft.AspNetCore.Mvc;

namespace ApiGateway.Controllers
{
    /// <summary>
    /// API Gateway controller for Orders Service operations
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<OrdersController> _logger;

        public OrdersController(IHttpClientFactory httpClientFactory, ILogger<OrdersController> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        /// <summary>
        /// Creates new order through the Orders Service
        /// </summary>
        /// <param name="request">Order creation request containing user info</param>
        /// <returns>Order creation response from the Orders Service</returns>
        /// <response code="201">Order created successfully</response>
        /// <response code="400">Invalid request data</response>
        /// <response code="503">Orders service is unavailable</response>
        /// <response code="500">Server error occurred</response>
        [HttpPost]
        [Produces("application/json")]
        [ProducesResponseType(typeof(OrderResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status503ServiceUnavailable)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequest request)
        {
            _logger.LogInformation("Forwarding create order request");

            try
            {
                var httpClient = _httpClientFactory.CreateClient("OrdersService");
                var content = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");

                _logger.LogInformation("Forwarding create order request for user {UserId} with amount {Amount}", 
                    request.UserId, request.Amount);

                var response = await httpClient.PostAsync("/api/orders", content);
                var responseBody = await response.Content.ReadAsStringAsync();

                _logger.LogInformation("Orders service response status: {StatusCode}", response.StatusCode);

                if (response.IsSuccessStatusCode)
                {
                    var orderResponse = JsonSerializer.Deserialize<OrderResponse>(responseBody, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                    if (orderResponse == null)
                    {
                        _logger.LogError("Failed to deserialize order response");
                        return StatusCode(500, new { message = "Internal server error: Failed to deserialize order response" });
                    }
                    return CreatedAtAction(nameof(GetOrder), new { orderId = orderResponse.Id }, orderResponse);
                }

                return StatusCode((int)response.StatusCode, string.IsNullOrEmpty(responseBody)
                    ? new { message = $"Orders service returned status code {response.StatusCode}" }
                    : JsonSerializer.Deserialize<object>(responseBody));
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Connection to Orders Service failed");
                return StatusCode(503, new { message = "Orders service is currently unavailable" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error forwarding create order request");
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        /// <summary>
        /// Gets specific order by id
        /// </summary>
        /// <param name="orderId">The id of the order to get</param>
        /// <returns>Order details from the Orders Service</returns>
        /// <response code="200">Order gotten successfully</response>
        /// <response code="404">Order not found</response>
        /// <response code="503">Orders service is unavailable</response>
        /// <response code="500">Server error occurred</response>
        [HttpGet("{orderId}")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(OrderResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status503ServiceUnavailable)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetOrder(string orderId)
        {
            _logger.LogInformation("Forwarding get order request for order {OrderId}", orderId);

            try
            {
                var httpClient = _httpClientFactory.CreateClient("OrdersService");
                var response = await httpClient.GetAsync($"/api/orders/{orderId}");
                var responseBody = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var orderResponse = JsonSerializer.Deserialize<OrderResponse>(responseBody, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                    return Ok(orderResponse);
                }

                return StatusCode((int)response.StatusCode, string.IsNullOrEmpty(responseBody)
                    ? new { message = $"Orders service returned status code {response.StatusCode}" }
                    : JsonSerializer.Deserialize<object>(responseBody));
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Connection to Orders Service failed");
                return StatusCode(503, new { message = "Orders service is currently unavailable" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error forwarding get order request for order {OrderId}", orderId);
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        /// <summary>
        /// Gets all orders for user
        /// </summary>
        /// <param name="userId">The id of the user</param>
        /// <param name="page">Page number for pagination (default=1)</param>
        /// <param name="pageSize">Number of orders per page (default=10)</param>
        /// <returns>List of orders from the Orders Service</returns>
        /// <response code="200">Orders gotten successfully</response>
        /// <response code="404">User not found or has no orders</response>
        /// <response code="503">Orders service is unavailable</response>
        /// <response code="500">Server error occurred</response>
        [HttpGet("user/{userId}")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(OrderListResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status503ServiceUnavailable)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetUserOrders(string userId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            _logger.LogInformation("Forwarding get user orders request for user {UserId}", userId);

            try
            {
                var httpClient = _httpClientFactory.CreateClient("OrdersService");
                var response = await httpClient.GetAsync($"/api/orders/user/{userId}?page={page}&pageSize={pageSize}");
                var responseBody = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var orderListResponse = JsonSerializer.Deserialize<OrderListResponse>(responseBody, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                    return Ok(orderListResponse);
                }

                return StatusCode((int)response.StatusCode, string.IsNullOrEmpty(responseBody)
                    ? new { message = $"Orders service returned status code {response.StatusCode}" }
                    : JsonSerializer.Deserialize<object>(responseBody));
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Connection to Orders Service failed");
                return StatusCode(503, new { message = "Orders service is currently unavailable" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error forwarding get user orders request for user {UserId}", userId);
                return StatusCode(500, new { message = "Internal server error" });
            }
        }
    }
}
