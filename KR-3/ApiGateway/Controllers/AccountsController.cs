using System.Text;
using System.Text.Json;
using ApiGateway.Models;
using Microsoft.AspNetCore.Mvc;

namespace ApiGateway.Controllers
{
    /// <summary>
    /// API Gateway controller for Account and Payment Service operations
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class AccountsController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<AccountsController> _logger;

        public AccountsController(IHttpClientFactory httpClientFactory, ILogger<AccountsController> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        /// <summary>
        /// Creates new account through the Payment Service
        /// </summary>
        /// <param name="request">Account creation request containing user info</param>
        /// <returns>Account creation response from the Payment Service</returns>
        /// <response code="201">Account created successfully</response>
        /// <response code="400">Invalid request data or account already exists</response>
        /// <response code="503">Payment service is unavailable</response>
        /// <response code="500">Server error occurred</response>
        [HttpPost]
        [Produces("application/json")]
        [ProducesResponseType(typeof(AccountResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status503ServiceUnavailable)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateAccount([FromBody] CreateAccountRequest request)
        {
            _logger.LogInformation("Forwarding create account request");

            try
            {
                var httpClient = _httpClientFactory.CreateClient("PaymentsService");
                var content = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");

                var response = await httpClient.PostAsync("/api/accounts", content);
                var responseBody = await response.Content.ReadAsStringAsync();

                _logger.LogInformation("Payment service response status: {StatusCode}", response.StatusCode);

                if (response.IsSuccessStatusCode)
                {
                    var accountResponse = JsonSerializer.Deserialize<AccountResponse>(responseBody, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                    if (accountResponse == null)
                    {
                        _logger.LogError("Deserialized AccountResponse is null");
                        return StatusCode(500, new { message = "Failed to parse account creation response" });
                    }
                    return CreatedAtAction(nameof(GetAccount), new { userId = accountResponse.UserId }, accountResponse);
                }

                return StatusCode((int)response.StatusCode, string.IsNullOrEmpty(responseBody)
                    ? new { message = $"Payment service returned status code {response.StatusCode}" }
                    : JsonSerializer.Deserialize<object>(responseBody));
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Connection to Payment Service failed");
                return StatusCode(503, new { message = "Payment service is currently unavailable" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error forwarding create account request");
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        /// <summary>
        /// Gets user's account information
        /// </summary>
        /// <param name="userId">The id of the user</param>
        /// <returns>Account information from the Payment Service</returns>
        /// <response code="200">Account gotten successfully</response>
        /// <response code="404">Account not found</response>
        /// <response code="503">Payment service is unavailable</response>
        /// <response code="500">Server error occurred</response>
        [HttpGet("{userId}")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(BalanceResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status503ServiceUnavailable)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAccount(string userId)
        {
            _logger.LogInformation("Forwarding get account request for user {UserId}", userId);

            try
            {
                var httpClient = _httpClientFactory.CreateClient("PaymentsService");
                var response = await httpClient.GetAsync($"/api/accounts/{userId}");
                var responseBody = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var balanceResponse = JsonSerializer.Deserialize<BalanceResponse>(responseBody, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                    return Ok(balanceResponse);
                }

                return StatusCode((int)response.StatusCode, string.IsNullOrEmpty(responseBody)
                    ? new { message = $"Payment service returned status code {response.StatusCode}" }
                    : JsonSerializer.Deserialize<object>(responseBody));
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Connection to Payment Service failed");
                return StatusCode(503, new { message = "Payment service is currently unavailable" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error forwarding get account request for user {UserId}", userId);
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        /// <summary>
        /// Deposits funds to user's account
        /// </summary>
        /// <param name="request">Deposit request containing user id and amount</param>
        /// <returns>Transaction response from the Payment Service</returns>
        /// <response code="200">Deposit successfully</response>
        /// <response code="400">Invalid request data</response>
        /// <response code="404">Account not found</response>
        /// <response code="503">Payment service is unavailable</response>
        /// <response code="500">Server error occurred</response>
        [HttpPost("deposit")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status503ServiceUnavailable)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Deposit([FromBody] DepositRequest request)
        {
            _logger.LogInformation("Forwarding deposit request");

            try
            {
                var httpClient = _httpClientFactory.CreateClient("PaymentsService");
                var content = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");

                _logger.LogInformation("Forwarding deposit request for user {UserId} with amount {Amount}", 
                    request.UserId, request.Amount);

                var response = await httpClient.PostAsync("/api/accounts/deposit", content);
                var responseBody = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var transactionResponse = JsonSerializer.Deserialize<object>(responseBody, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                    return Ok(transactionResponse);
                }

                return StatusCode((int)response.StatusCode, string.IsNullOrEmpty(responseBody)
                    ? new { message = $"Payment service returned status code {response.StatusCode}" }
                    : JsonSerializer.Deserialize<object>(responseBody));
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Connection to Payment Service failed");
                return StatusCode(503, new { message = "Payment service is currently unavailable" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error forwarding deposit request");
                return StatusCode(500, new { message = "Internal server error" });
            }
        }
    }
}
