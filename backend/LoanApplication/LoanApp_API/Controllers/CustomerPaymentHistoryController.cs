
using LoanApp_API.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LoanApp_API.Controllers
{
    [ApiController]
    [Route("api/customer/payment-history")]
    [Authorize]
    public class PaymentHistoryController : ControllerBase
    {
        private readonly IPaymentHistoryRepository _paymentHistoryRepository;

        public PaymentHistoryController(
            IPaymentHistoryRepository paymentHistoryRepository)
        {
            _paymentHistoryRepository = paymentHistoryRepository;
        }

        private int? GetCustomerId()
        {
            string? customerId =
                User.FindFirst("CustomerId")?.Value ??
                User.FindFirst("customerId")?.Value ??
                User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            return int.TryParse(customerId, out int id) ? id : null;
        }

        // GET: api/customer/payment-history?loanAccountId=1&months=3
        [HttpGet]
        public async Task<IActionResult> GetPaymentHistory(
            [FromQuery] int? loanAccountId,
            [FromQuery] int months = 0)
        {
            var customerId = GetCustomerId();

            if (customerId == null)
                return Unauthorized(new { message = "Customer ID not found in token." });

            if (months < 0)
                return BadRequest(new { message = "Months cannot be negative." });

            var payments =
                await _paymentHistoryRepository.GetPaymentHistory(
                    customerId.Value,
                    loanAccountId,
                    months);

            return Ok(payments);
        }
    }
}
