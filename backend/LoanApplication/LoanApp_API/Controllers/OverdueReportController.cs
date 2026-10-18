using LoanApp_API.Application.Interfaces.Reports;
using Microsoft.AspNetCore.Mvc;

namespace LoanApp_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OverdueReportController : ControllerBase
    {
        IOverdueReport service;

        public OverdueReportController(IOverdueReport service)
        {
            this.service = service;
        }

        [HttpGet("fetchOverdueReport")]
        public async Task<IActionResult> FetchOverdueReport([FromQuery] DateOnly? asOnDate)
        {
            var data = await service.fetchOverdueReport(asOnDate);
            return Ok(data);
        }
    }
}
