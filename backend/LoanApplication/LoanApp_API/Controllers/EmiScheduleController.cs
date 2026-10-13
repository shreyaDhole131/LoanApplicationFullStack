using Hangfire;
using LoanApp_API.Application.Interfaces;
using LoanApp_API.Infrastructure.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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

        [HttpPost]
        [Route("generate/{loanAccountId}")]
        public IActionResult GenerateEMISchedule(int loanAccountId)
        {
            var jobId = BackgroundJob.Enqueue<IEmiScheduleService>(
                service => service.GenerateEMISchedule(loanAccountId)
            );

            return Ok(new
            {
                Message = "EMI schedule generation started successfully.",
                LoanAccountId = loanAccountId,
                JobId = jobId
            });
        }

      

        [HttpGet]
        [Route("EmiSchedule/{loanAccountId}")]
        public async Task<IActionResult> GetEMISchedule(int loanAccountId)
        {
            var result = await service.GetEMISchedule(loanAccountId);

            return Ok(result);
        }
    }
}
