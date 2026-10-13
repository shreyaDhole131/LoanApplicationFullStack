using LoanApp_API.Application.Interfaces.Reports;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LoanApp_API.Controllers.Reports
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmiReportController : ControllerBase
    {
        IEmiReport service;
        public EmiReportController(IEmiReport service)
        {
            this.service = service;
        }

        [HttpGet("fetchEmiReport")]
        public async Task<IActionResult> fetchEmiReport([FromQuery] DateOnly? fromDate, [FromQuery] DateOnly? toDate)
        {
            if (fromDate.HasValue && toDate.HasValue && fromDate > toDate)
                return BadRequest("fromDate cannot be later than toDate");

            var data = await service.fetchEmiReport(fromDate, toDate);
            return Ok(data);
        }
    }
}
