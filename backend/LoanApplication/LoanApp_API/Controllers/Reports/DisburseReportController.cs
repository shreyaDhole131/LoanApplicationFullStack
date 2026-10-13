using LoanApp_API.Application.Interfaces.Reports;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LoanApp_API.Controllers.Reports
{
    [Route("api/[controller]")]
    [ApiController]
    public class DisburseReportController : ControllerBase
    {
        IDisburseReport service;

        public DisburseReportController(IDisburseReport service)
        {
            this.service = service;
        }

        [HttpGet("fetchDisburseReport")]

        public async Task<IActionResult> GetDisburseReport([FromQuery] DateOnly? fromDate, [FromQuery] DateOnly? toDate)
        {
            if (fromDate.HasValue && toDate.HasValue && fromDate > toDate)
                return BadRequest("fromDate cannot be later than toDate");
            var data = await service.fetchDisbursmentReport(fromDate,toDate);
            return Ok(data);
        }
    }
}
