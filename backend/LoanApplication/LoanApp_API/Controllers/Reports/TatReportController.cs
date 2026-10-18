using LoanApp_API.Application.Interfaces.Reports;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LoanApp_API.Controllers.Reports
{
    [Route("api/[controller]")]
    [ApiController]
    public class TatReportController : ControllerBase
    {
        private readonly ITatReport service;

        public TatReportController(ITatReport service)
        {
            this.service = service;
        }

        [HttpGet("fetchTatReport")]
        public async Task<IActionResult> fetchTatReport([FromQuery] DateOnly? fromDate, [FromQuery] DateOnly? toDate)
        {
            var data = await service.fetchTatReport(fromDate, toDate);
            return Ok(data);

        }
    }
}
