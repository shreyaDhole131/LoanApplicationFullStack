using LoanApp_API.Infrastructure.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;


namespace LoanApp_API.Controllers.Reports
{
    [Route("api/[controller]")]
    [ApiController]
    public class CibilReportController : ControllerBase
    {
        CibiReportService service;

        public CibilReportController(CibiReportService service)
        {
            this.service = service;
        }

        [HttpGet("fetchCibilReports")]
        public async Task<IActionResult> fetchCibilReports([FromQuery]DateOnly? fromDate,[FromQuery] DateOnly? toDate)
        {
            if (fromDate.HasValue && toDate.HasValue && fromDate > toDate)
                return BadRequest("fromDate cannot be later than toDate");
            var data = await service.fetchCibilReport(fromDate,toDate);
            return Ok(data);

        }

    }
}
