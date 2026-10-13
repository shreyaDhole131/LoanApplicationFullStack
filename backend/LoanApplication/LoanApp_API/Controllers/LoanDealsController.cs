using LoanApp_API.Application.DTO;
using LoanApp_API.Application.Interfaces;
using LoanApp_API.Infrastructure.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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

        [HttpPost]
        [Route("AddLoan")]
        public async Task<IActionResult> AddLoan([FromBody] LoanDealDTO dto)
        {
            var customerId = 1;
            await service.applyLoan(dto, customerId);
            return Ok(new { message = "loan added successfully" });
        }
    }
}
