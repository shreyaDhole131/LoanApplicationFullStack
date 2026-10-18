using LoanApp_API.Application.Interfaces;
using LoanApp_API.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LoanApp_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoanAccountController : ControllerBase
    {
        ILoanAccountService service;
        public LoanAccountController(ILoanAccountService service)
        {
            this.service = service; 
        }

        

        [HttpGet]
        [Route("getA")]
        public async Task<IActionResult> GetAllLoanAccounts()
        {
            try
            {
                var loanAccounts = await service.FetchAllLoanAccounts();

                return Ok(loanAccounts);
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
        }

        [Authorize]
        [HttpGet("my-loans")]
        public async Task<IActionResult> GetMyLoanAccounts()
        {
            var customerIdClaim = User.FindFirst("CustomerId")?.Value;

            if (string.IsNullOrEmpty(customerIdClaim))
            {
                return Unauthorized("CustomerId not found in token.");
            }

            int customerId = int.Parse(customerIdClaim);

            var loanAccounts =
                await service.FetchLoanAccountById(customerId);

            if (loanAccounts == null || loanAccounts.Count == 0)
            {
                return NotFound("No loan accounts found.");
            }

            return Ok(loanAccounts);
        }

        [Authorize]
        [HttpGet("getById/{id}")]
        public async Task<IActionResult> GetLoanAccountById(int id)
        {
            var loanAccount = await service.FetchLoanAccountDetailsById(id);

            if (loanAccount == null)
            {
                return NotFound("Loan account not found.");
            }

            return Ok(loanAccount);
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchLoanAccounts(
        [FromQuery] string search)
        {
            if (string.IsNullOrWhiteSpace(search))
            {
                return Ok(await service.FetchAllLoanAccounts());
            }

            var result = await service.SearchLoanAccounts(search);

            return Ok(result);
        }
    }
}
