using LoanApp_API.Application.DTO;
using LoanApp_API.Application.Interfaces;
using LoanApp_API.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LoanApp_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoanDealsController : ControllerBase
    {
        ILoanDeals service;

        public LoanDealsController(ILoanDeals service)
        {
            this.service = service;
        }
        [Authorize]
        [HttpPost]
        [Route("AddLoan")]
        public async Task<IActionResult> AddLoan([FromBody] LoanDealDTO dto)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
                var data = await service.applyLoan(dto, userId);
                return Ok(new { message = "loan added successfully", mydata = data });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { ex.Message });

            }
        }

        [Authorize]
        [HttpPost("Preview")]
        public IActionResult Preview([FromBody] LoanPreviewRequestDTO dto)
        {
            try
            {
                var result = service.CalculatePreview(dto.LoanAmount, dto.TenureMonths);
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { ex.Message });

            }
        }
    }
}
