using LoanApp_API.Application.Interfaces.Reports;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LoanApp_API.Controllers.Reports
{
    [Route("api/[controller]")]
    [ApiController]
    public class ScoreReportController : ControllerBase
    {
        IScoreReport service;

        public ScoreReportController(IScoreReport service)
        {
            this.service = service;
        }

        [HttpGet("fetchScoreReport")]
        public async Task<IActionResult> fetchScoreReport([FromQuery] DateOnly? fromDate, [FromQuery] DateOnly? toDate)
        {
            if (fromDate.HasValue && toDate.HasValue && fromDate > toDate)
                return BadRequest("fromDate cannot be later than toDate");
            var data = await service.fetchScoreCardReport(fromDate, toDate);
            return Ok(data);
        }
    }
}
