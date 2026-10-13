using LoanApp_API.Application.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LoanApp_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PendingDealsController : ControllerBase
    {
        IPendingDeals service;

        public PendingDealsController(IPendingDeals service)
        {
            this.service = service;
        }

        [HttpGet("FetchCustomerProfile")]
        public async Task<IActionResult> fetchCustomerProfile(int dealId)
        {
            var data =await service.fetchCustomerProfile(dealId);
            return Ok(data);

        }

        [HttpGet("PendingDeals")]

        public async Task<IActionResult> fetchPendingDeals()
        {
            var data = await service.fetchPendingDeals();
            return Ok(data);
        }

        [HttpGet("SortingPending")]

        public async Task<IActionResult> sortPendingDeals([FromQuery]string? LoanType,
            [FromQuery] decimal? MinAmount, [FromQuery] decimal? MaxAmount, [FromQuery] string? dateFilter, [FromQuery] DateOnly? FromFilter, [FromQuery] DateOnly? ToFilter)
        {
            var data = await service.sortPendingDeals(LoanType,MinAmount,MaxAmount,dateFilter,FromFilter,ToFilter);
            return Ok(data);
        }

        [HttpPost("{dealId}/Review")]
        public async Task<IActionResult> ReviewDeal(int dealId,string status)
        {
            if (status != "Approved" && status != "Rejected")
                return BadRequest(new { message = "Status must be rejected or approved" });

            await service.reviewDeal(dealId, status);
            return Ok(new { message = $"Deal {status.ToLower()}." });
        }
    }
}
