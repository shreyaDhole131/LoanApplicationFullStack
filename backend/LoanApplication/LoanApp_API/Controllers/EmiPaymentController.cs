
using LoanApp_API.Application.DTO.EmiPayment;
using LoanApp_API.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LoanApp_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class EmiPaymentController : ControllerBase
    {
        private readonly IEmiPaymentRepository _repository;

        public EmiPaymentController(IEmiPaymentRepository repository)
        {
            _repository = repository;
        }

        // Get EMI payment details
        [HttpGet("details/{emiScheduleId}")]
        public async Task<IActionResult> GetPaymentDetails(int emiScheduleId)
        {
            int customerId = GetCustomerId();

            if (customerId <= 0)
                return Unauthorized("Customer ID not found in token");

            var result = await _repository.GetPaymentDetails(
                emiScheduleId, customerId);

            if (result == null)
                return NotFound("EMI not found or already paid");

            return Ok(result);
        }

        // Create Razorpay order
        [HttpPost("create-order/{emiScheduleId}")]
        public async Task<IActionResult> CreatePaymentOrder(int emiScheduleId)
        {
            int customerId = GetCustomerId();

            if (customerId <= 0)
                return Unauthorized("Customer ID not found in token");

            var result = await _repository.CreatePaymentOrder(
                emiScheduleId, customerId);

            if (!result.Success)
                return BadRequest(result.Message);

            return Ok(result.Data);
        }

        // Verify Razorpay payment
        [HttpPost("verify-payment")]
        public async Task<IActionResult> VerifyPayment(
            VerifyEmiPaymentDto dto)
        {
            int customerId = GetCustomerId();

            if (customerId <= 0)
                return Unauthorized("Customer ID not found in token");

            var result = await _repository.VerifyPayment(dto, customerId);

            if (!result.Success)
                return BadRequest(result.Message);

            return Ok(new { Message = result.Message });
        }

        // Get customer ID from JWT
        private int GetCustomerId()
        {
            string? id =
                User.FindFirst("CustomerId")?.Value ??
                User.FindFirst(ClaimTypes.NameIdentifier)?.Value ??
                User.FindFirst("sub")?.Value;

            return int.TryParse(id, out int customerId)
                ? customerId
                : 0;
        }
    }
}
