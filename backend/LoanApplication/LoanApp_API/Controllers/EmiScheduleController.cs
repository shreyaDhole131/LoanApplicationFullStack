using Hangfire;
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
    public class EmiScheduleController : ControllerBase
    {
        IEmiScheduleService service;    
        public EmiScheduleController(IEmiScheduleService service)
        {
            this.service = service;
        }

        

        [Authorize]
        [HttpGet("MyLoanAccounts")]
        public async Task<IActionResult> GetMyLoanAccounts()
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;

            if (string.IsNullOrEmpty(email))
                return Unauthorized("Email not found in token.");

            var result = await service.GetMyLoanAccounts(email);

            return Ok(result);
        }

        [Authorize]
        [HttpGet("MyEmiSchedule/{loanAccountId}")]
        public async Task<IActionResult> GetMyEMISchedule(int loanAccountId)
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;

            if (string.IsNullOrEmpty(email))
                return Unauthorized("Email not found in token.");

            var result = await service.GetEMISchedule(
                loanAccountId,
                email);

            return Ok(result);
        }

    }
}
