using Microsoft.AspNetCore.Mvc;
using PaymentsService.Models;
using PaymentsService.Services;

namespace PaymentsService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountsController : ControllerBase
    {
        private readonly IAccountService _accountService;
        private readonly ILogger<AccountsController> _logger;

        public AccountsController(
            IAccountService accountService,
            ILogger<AccountsController> logger)
        {
            _accountService = accountService;
            _logger = logger;
        }

        [HttpPost]
        [ProducesResponseType(typeof(AccountResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateAccount([FromBody] CreateAccountRequest request)
        {
            try
            {
                var account = await _accountService.CreateAccountAsync(request.UserId);
                return CreatedAtAction(nameof(GetBalance), new { userId = request.UserId }, account);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Failed to create account for user {UserId}", request.UserId);
                return BadRequest(new ErrorResponse(ex.Message, StatusCodes.Status400BadRequest));
            }
        }

        [HttpGet("{userId}")]
        [ProducesResponseType(typeof(BalanceResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetBalance(Guid userId)
        {
            try
            {
                var balance = await _accountService.GetBalanceAsync(userId);
                return Ok(balance);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Account not found for user {UserId}", userId);
                return NotFound(new ErrorResponse(ex.Message, StatusCodes.Status404NotFound));
            }
        }

        [HttpPost("deposit")]
        [ProducesResponseType(typeof(TransactionResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Deposit([FromBody] DepositRequest request)
        {
            try
            {
                var transaction = await _accountService.DepositAsync(
                    request.UserId,
                    request.Amount,
                    request.Description ?? "Deposit");

                return Ok(transaction);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Account not found for user {UserId}", request.UserId);
                return NotFound(new ErrorResponse(ex.Message, StatusCodes.Status404NotFound));
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid deposit request for user {UserId}", request.UserId);
                return BadRequest(new ErrorResponse(ex.Message, StatusCodes.Status400BadRequest));
            }
        }
    }
}
