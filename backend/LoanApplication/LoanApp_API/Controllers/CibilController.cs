
using LoanApp_API.Application.DTO.Common;
using LoanApp_API.Application.DTO.Cibil;
using LoanApp_API.Application.Interfaces;
using LoanApp_API.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace LoanApp_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CibilController : ControllerBase
    {
        private readonly ICibilService service;
        private readonly AppDbContext context;

        public CibilController(
            ICibilService service,
            AppDbContext context)
        {
            this.service = service;
            this.context = context;
        }

        // Generate CIBIL score for the logged-in customer
        [Authorize(Roles = "1")]
        [HttpPost("GenerateMyCibil")]
        public async Task<IActionResult> GenerateMyCibil()
        {
            var userIdValue = User.FindFirstValue(
                ClaimTypes.NameIdentifier);

            if (!int.TryParse(userIdValue, out int userId))
                return Unauthorized();

            var user = await context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.UserId == userId);

            if (user == null || user.CustomerId == null)
            {
                return BadRequest(new ResponseWrapper<CibilScoreResponseDto>
                {
                    Success = false,
                    Message = "Customer account not found.",
                    Data = null
                });
            }

            var data = await service.GenerateScoreAsync(
                user.CustomerId.Value);

            if (data == null)
            {
                return NotFound(new ResponseWrapper<CibilScoreResponseDto>
                {
                    Success = false,
                    Message = "Customer not found.",
                    Data = null
                });
            }

            return Ok(new ResponseWrapper<CibilScoreResponseDto>
            {
                Success = true,
                Message = "CIBIL score generated successfully.",
                Data = data
            });
        }

        // Get the saved CIBIL report of the logged-in customer
        [Authorize(Roles = "1")]
        [HttpGet("MyCibil")]
        public async Task<IActionResult> GetMyCibil()
        {
            var userIdValue = User.FindFirstValue(
                ClaimTypes.NameIdentifier);

            if (!int.TryParse(userIdValue, out int userId))
                return Unauthorized();

            var user = await context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.UserId == userId);

            if (user == null || user.CustomerId == null)
            {
                return BadRequest(new ResponseWrapper<CibilScoreResponseDto>
                {
                    Success = false,
                    Message = "Customer account not found.",
                    Data = null
                });
            }

            var data = await service.GetCibilReportAsync(
                user.CustomerId.Value);

            if (data == null)
            {
                return NotFound(new ResponseWrapper<CibilScoreResponseDto>
                {
                    Success = false,
                    Message = "CIBIL report not found. Generate the score first.",
                    Data = null
                });
            }

            return Ok(new ResponseWrapper<CibilScoreResponseDto>
            {
                Success = true,
                Message = "CIBIL report fetched successfully.",
                Data = data
            });
        }

        // Existing endpoint: Generate CIBIL Score by Customer ID
        [HttpPost("GenerateCibil/{customerId}")]
        public async Task<IActionResult> GenerateScore(int customerId)
        {
            if (customerId <= 0)
            {
                return BadRequest(new ResponseWrapper<CibilScoreResponseDto>
                {
                    Success = false,
                    Message = "Invalid customer ID.",
                    Data = null
                });
            }

            var data = await service.GenerateScoreAsync(customerId);

            if (data == null)
            {
                return NotFound(new ResponseWrapper<CibilScoreResponseDto>
                {
                    Success = false,
                    Message = "Customer not found.",
                    Data = null
                });
            }

            return Ok(new ResponseWrapper<CibilScoreResponseDto>
            {
                Success = true,
                Message = "CIBIL Score Generated Successfully",
                Data = data
            });
        }

        // Existing endpoint: Get CIBIL Report by Customer ID
        [HttpGet("GetCibil/{customerId}")]
        public async Task<IActionResult> GetCibilReport(int customerId)
        {
            if (customerId <= 0)
            {
                return BadRequest(new ResponseWrapper<CibilScoreResponseDto>
                {
                    Success = false,
                    Message = "Invalid customer ID.",
                    Data = null
                });
            }

            var data = await service.GetCibilReportAsync(customerId);

            if (data == null)
            {
                return NotFound(new ResponseWrapper<CibilScoreResponseDto>
                {
                    Success = false,
                    Message = "CIBIL Report Not Found.",
                    Data = null
                });
            }

            return Ok(new ResponseWrapper<CibilScoreResponseDto>
            {
                Success = true,
                Message = "CIBIL Report Fetched Successfully",
                Data = data
            });
        }

        // Existing endpoint: Get All CIBIL Reports
        [HttpGet("GetAllCibil")]
        public async Task<IActionResult> GetAllCibilReports()
        {
            var data = await service.GetAllCibilReportsAsync();

            return Ok(new ResponseWrapper<List<CibilScoreResponseDto>>
            {
                Success = true,
                Message = "CIBIL Reports Fetched Successfully",
                Data = data
            });
        }

        // Existing endpoint: Get Customers
        [HttpGet("customers")]
        public async Task<IActionResult> GetCustomers()
        {
            var data = await service.GetCustomersAsync();

            return Ok(new ResponseWrapper<List<CibilCustomerDto>>
            {
                Success = true,
                Message = "Customers fetched successfully.",
                Data = data
            });
        }
    }
}
