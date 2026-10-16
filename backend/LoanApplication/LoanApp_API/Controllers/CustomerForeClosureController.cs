
using LoanApp_API.Application.DTO.ForeClosure;
using LoanApp_API.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LoanApp_API.Controllers
{
    [ApiController]
    [Route("api/customer/foreclosure")]
    [Authorize]
    public class CustomerForeClosureController : ControllerBase
    {
        private readonly IForeClosureRepository _foreClosureRepository;

        public CustomerForeClosureController(
            IForeClosureRepository foreClosureRepository)
        {
            _foreClosureRepository = foreClosureRepository;
        }

        private int? GetCustomerId()
        {
            string? customerId =
                User.FindFirst("CustomerId")?.Value ??
                User.FindFirst("customerId")?.Value ??
                User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            return int.TryParse(customerId, out int id) ? id : null;
        }

        // GET: api/customer/foreclosure/loans
        [HttpGet("loans")]
        public async Task<IActionResult> GetCustomerLoans()
        {
            var customerId = GetCustomerId();

            if (customerId == null)
                return Unauthorized(new { message = "Customer ID not found in token." });

            var loans = await _foreClosureRepository.GetCustomerLoans(customerId.Value);

            return Ok(loans);
        }

        // GET: api/customer/foreclosure/summary/5
        [HttpGet("summary/{loanAccountId}")]
        public async Task<IActionResult> GetSummary(int loanAccountId)
        {
            var customerId = GetCustomerId();

            if (customerId == null)
                return Unauthorized(new { message = "Customer ID not found in token." });

            // Ensure the loan belongs to the logged-in customer
            var loans = await _foreClosureRepository.GetCustomerLoans(customerId.Value);

            if (!loans.Any(x => x.LoanAccountId == loanAccountId))
                return NotFound(new { message = "Loan not found." });

            var summary = await _foreClosureRepository.GetSummary(loanAccountId);

            if (summary == null)
                return NotFound(new { message = "Loan summary not found." });

            return Ok(summary);
        }

        // POST: api/customer/foreclosure
        [HttpPost]
        public async Task<IActionResult> SubmitRequest(
            [FromBody] ForeClosureDto dto)
        {
            var customerId = GetCustomerId();

            if (customerId == null)
                return Unauthorized(new { message = "Customer ID not found in token." });

            // Take the customer ID from the token, not the request body
            dto.CustomerId = customerId.Value;

            var (success, message) =
                await _foreClosureRepository.SubmitRequest(dto);

            if (!success)
                return BadRequest(new { message });

            return Ok(new { message });
        }

        // GET: api/customer/foreclosure/requests
        [HttpGet("requests")]
        public async Task<IActionResult> GetMyRequests()
        {
            var customerId = GetCustomerId();

            if (customerId == null)
                return Unauthorized(new { message = "Customer ID not found in token." });

            var requests =
                await _foreClosureRepository.GetMyRequests(customerId.Value);

            return Ok(requests);
        }

        // POST: api/customer/foreclosure/payment/create-order/5
        [HttpPost("payment/create-order/{requestId}")]
        public async Task<IActionResult> CreatePaymentOrder(int requestId)
        {
            var customerId = GetCustomerId();

            if (customerId == null)
                return Unauthorized(new { message = "Customer ID not found in token." });

            var (success, message, data) =
                await _foreClosureRepository.CreatePaymentOrder(requestId, customerId.Value);

            if (!success)
                return BadRequest(new { message });

            return Ok(data);
        }

        // POST: api/customer/foreclosure/payment/verify
        [HttpPost("payment/verify")]
        public async Task<IActionResult> VerifyPayment(
            [FromBody] VerifyForeClosurePaymentDto dto)
        {
            var customerId = GetCustomerId();

            if (customerId == null)
                return Unauthorized(new { message = "Customer ID not found in token." });

            var (success, message) =
                await _foreClosureRepository.VerifyPayment(dto, customerId.Value);

            if (!success)
                return BadRequest(new { message });

            return Ok(new { message });
        }
    }
}
