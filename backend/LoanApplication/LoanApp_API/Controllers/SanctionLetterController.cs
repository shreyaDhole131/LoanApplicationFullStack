using LoanApp_API.Application.DTO;
using LoanApp_API.Application.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LoanApp_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SanctionLetterController : ControllerBase
    {
        ISanctionLetter service;

        IDisbursement dservice;

        public SanctionLetterController(ISanctionLetter service, IDisbursement dservice)
        {
            this.service = service;
            this.dservice = dservice;
        }

        [HttpGet("pending-customers")]
        public async Task<IActionResult> GetPendingCustomers()
        {
            var data = await service.fetchCustomers();
            return Ok(data);
        }

        [HttpGet("prefill/{dealId}")]
        public async Task<IActionResult> GetPrefill(int dealId)
        {
            var dto = await service.fetchSanctionDetails(dealId);
            if (dto == null) return NotFound(new { message = $"Deal {dealId} not found." });
            return Ok(dto);
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateSanctionLetter([FromBody] CreateSanctionDTO dto)
        {
            if (dto.DealId <= 0)
                return BadRequest(new { message = "DealId is required." });

            await service.AddSanctionLetter(dto);
            return Ok(new { message = "Sanction letter generated successfully." });
        }

        [HttpGet("grid")]
        public async Task<IActionResult> GetSanctionGrid([FromQuery] string? name,
            [FromQuery] string? fromDate,
            [FromQuery] string? toDate,
            [FromQuery] string? filter,
            [FromQuery] string? sort)
        {
            var data = await service.fetchSanctionsGrid(name, fromDate, toDate, filter, sort);
            return Ok(data);
        }

        [HttpGet("fetchDisburse")]

        public async Task<IActionResult> GetDisburse(int dealId)
        {
            var data = await dservice.fetchDisbursePrefill(dealId);
            return Ok(data);
        }

        [HttpPost("PostDisburse")]
        public async Task<IActionResult> createDisburse([FromBody] CreateDisbursementDTO dto)
        {
            await dservice.ApplyDisbursement(dto);
            return Ok(new { message = " Amount Disbursed successfully." });

        }
    }
}
