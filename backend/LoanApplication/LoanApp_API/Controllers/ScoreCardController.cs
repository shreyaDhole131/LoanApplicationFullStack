
using LoanApp_API.Application.DTO.Common;
using LoanApp_API.Application.DTO.ScoreCard;
using LoanApp_API.Application.Interfaces;
using LoanApp_API.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace LoanApp_API.Controllers
{
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
        public async Task<ActionResult<ResponseWrapper<List<ScoreCardCustomerDto>>>>
            GetCustomers()
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
        public async Task<ActionResult<ResponseWrapper<ScoreCard>>>
            GenerateScoreCard(int customerId)
        {
            if (customerId <= 0)
            {
                return BadRequest(new ResponseWrapper<ScoreCard>
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
                return NotFound(new ResponseWrapper<ScoreCard>
                {
                    Success = false,
                    Message = "Customer or CIBIL report not found.",
                    Data = null
                });
            }

            return Ok(new ResponseWrapper<ScoreCard>
            {
                Success = true,
                Message = "Scorecard generated successfully.",
                Data = scoreCard
            });
        }

        // GET: api/ScoreCard/1
        [HttpGet("{customerId:int}")]
        public async Task<ActionResult<ResponseWrapper<ScoreCard>>>
            GetScoreCard(int customerId)
        {
            if (customerId <= 0)
            {
                return BadRequest(new ResponseWrapper<ScoreCard>
                {
                    Success = false,
                    Message = "Invalid customer ID.",
                    Data = null
                });
            }

            var scoreCard = await service.GetScoreCardAsync(customerId);

            if (scoreCard == null)
            {
                return NotFound(new ResponseWrapper<ScoreCard>
                {
                    Success = false,
                    Message = "Scorecard not found. Generate it first.",
                    Data = null
                });
            }

            return Ok(new ResponseWrapper<ScoreCard>
            {
                Success = true,
                Message = "Scorecard retrieved successfully.",
                Data = scoreCard
            });
        }

        // GET: api/ScoreCard
        [HttpGet]
        public async Task<ActionResult<ResponseWrapper<List<ScoreCard>>>>
            GetAllScoreCards()
        {
            var scoreCards = await service.GetAllScoreCardsAsync();

            return Ok(new ResponseWrapper<List<ScoreCard>>
            {
                Success = true,
                Message = "Scorecards retrieved successfully.",
                Data = scoreCards
            });
        }
    }
}
