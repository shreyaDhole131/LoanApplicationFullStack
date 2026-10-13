using LoanApp_API.Infrastructure.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LoanApp_API.Controllers.Reports
{
    [Route("api/[controller]")]
    [ApiController]
    public class PrePaymentController : ControllerBase
    {
        PrepaymentReportService service;

        public PrePaymentController(PrepaymentReportService service)
        {
            this.service = service;
        }

        [HttpGet("fetchPrepayment")]
        public async Task<IActionResult> fetchPrepayment([FromQuery] DateOnly? fromDate, [FromQuery] DateOnly? toDate)
        {
            if (fromDate.HasValue && toDate.HasValue && fromDate > toDate)
                return BadRequest("fromDate cannot be later than toDate");
            var data = await service.fetchPrepaymentReport(fromDate, toDate);
            return Ok(data);
        }
    }
}
