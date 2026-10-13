using LoanApp_API.Application.Interfaces.Reports;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LoanApp_API.Controllers.Reports
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoanProtofolioController : ControllerBase
    {
        ILoanProtfolio service;

        public LoanProtofolioController(ILoanProtfolio service)
        {
            this.service = service;

        }

        [HttpGet("fetchProtofolioReport")]
        public async Task<IActionResult> fetchProtofolioReport(DateOnly? fromDate,DateOnly? toDate)
        {

            if (fromDate.HasValue && toDate.HasValue && fromDate > toDate)
                return BadRequest("fromDate cannot be later than toDate");
            var data = await service.fetchLoanProtofolio(fromDate,toDate);
            return Ok(data);

        }

    }
}
