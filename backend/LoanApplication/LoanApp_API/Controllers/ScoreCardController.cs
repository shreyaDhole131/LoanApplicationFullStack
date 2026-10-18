using LoanApp_API.Application.DTO.Common;
using LoanApp_API.Application.DTO.ScoreCard;
using LoanApp_API.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LoanApp_API.Controllers
{
    [Authorize(Roles = "2")]
    [Route("api/[controller]")]
    [ApiController]
    public class ScoreCardController : ControllerBase
    {
        private readonly IScoreCardService service;

        public ScoreCardController(IScoreCardService service)
        {
            this.service = service;
        }

        // GET: api/ScoreCard/customers
        [HttpGet("customers")]
        public async Task<ActionResult<
            ResponseWrapper<List<ScoreCardCustomerDto>>>> GetCustomers()
        {
            var customers = await service.GetCustomersAsync();

            return Ok(new ResponseWrapper<List<ScoreCardCustomerDto>>
            {
                Success = true,
                Message = "Customers retrieved successfully.",
                Data = customers
            });
        }

        // POST: api/ScoreCard/Generate/1
        [HttpPost("Generate/{customerId:int}")]
        public async Task<ActionResult<
            ResponseWrapper<ScoreCardDto>>> GenerateScoreCard(
                int customerId)
        {
            if (customerId <= 0)
            {
                return BadRequest(new ResponseWrapper<ScoreCardDto>
                {
                    Success = false,
                    Message = "Invalid customer ID.",
                    Data = null
                });
            }

            var scoreCard =
                await service.GenerateScoreCardAsync(customerId);

            if (scoreCard == null)
            {
                return NotFound(new ResponseWrapper<ScoreCardDto>
                {
                    Success = false,
                    Message = "Customer or CIBIL report not found.",
                    Data = null
                });
            }

            return Ok(new ResponseWrapper<ScoreCardDto>
            {
                Success = true,
                Message = "Scorecard generated successfully.",
                Data = scoreCard
            });
        }
    }
}