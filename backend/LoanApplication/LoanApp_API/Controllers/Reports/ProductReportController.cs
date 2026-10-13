using LoanApp_API.Application.Interfaces.Reports;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LoanApp_API.Controllers.Reports
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductReportController : ControllerBase
    {
        IProductWiseReport service;

        public ProductReportController(IProductWiseReport service)
        {
            this.service = service;
        }

        [HttpGet("fetchProductReport")]
        public async Task<IActionResult> fetchProductReport([FromQuery] DateOnly? fromDate, [FromQuery] DateOnly? toDate)
        {
            if (fromDate.HasValue && toDate.HasValue && fromDate > toDate)
                return BadRequest("fromDate cannot be later than toDate");
            var data = await service.fetchProductReport(fromDate, toDate);
            return Ok(data);
        }
    }
}
