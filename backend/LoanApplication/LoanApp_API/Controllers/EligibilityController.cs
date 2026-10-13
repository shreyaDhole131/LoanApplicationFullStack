
using LoanApp_API.Application.DTO.Common;
using LoanApp_API.Application.DTO.Eligibility;
using LoanApp_API.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LoanApp_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EligibilityController : ControllerBase
    {
        private readonly IEligibilityService service;

        public EligibilityController(IEligibilityService service)
        {
            this.service = service;
        }

        // GET: api/Eligibility/manual-review-customers
        [HttpGet("manual-review-customers")]
        public async Task<
            ActionResult<ResponseWrapper<List<EligibilityCustomerDto>>>>
            GetManualReviewCustomers()
        {
            var customers =
                await service.GetManualReviewCustomersAsync();

            return Ok(new ResponseWrapper<List<EligibilityCustomerDto>>
            {
                Success = true,
                Message = "Manual Review customers retrieved successfully.",
                Data = customers
            });
        }

        // POST: api/Eligibility/evaluate/12
        [HttpPost("evaluate/{customerId:int}")]
        public async Task<
            ActionResult<ResponseWrapper<EligibilityEvaluationDto>>>
            EvaluateAndSave(int customerId)
        {
            if (customerId <= 0)
            {
                return BadRequest(
                    new ResponseWrapper<EligibilityEvaluationDto>
                    {
                        Success = false,
                        Message = "Invalid customer ID.",
                        Data = null
                    });
            }

            var result = await service.EvaluateAndSaveAsync(customerId);

            if (result == null)
            {
                return NotFound(
                    new ResponseWrapper<EligibilityEvaluationDto>
                    {
                        Success = false,
                        Message = "Customer or CIBIL report not found.",
                        Data = null
                    });
            }

            string message =
                result.ApplicationStatus == "Manual Review" &&
                result.MonthlyIncome <= 0
                    ? "Eligibility status saved, but tenure calculation was skipped because monthly income is invalid."
                    : "Eligibility decision and applicable tenure results saved successfully.";

            return Ok(new ResponseWrapper<EligibilityEvaluationDto>
            {
                Success = true,
                Message = message,
                Data = result
            });
        }

        // GET: api/Eligibility/result/12
        [HttpGet("result/{customerId:int}")]
        public async Task<
            ActionResult<ResponseWrapper<EligibilityEvaluationDto>>>
            GetSavedResult(int customerId)
        {
            if (customerId <= 0)
            {
                return BadRequest(
                    new ResponseWrapper<EligibilityEvaluationDto>
                    {
                        Success = false,
                        Message = "Invalid customer ID.",
                        Data = null
                    });
            }

            var result = await service.GetSavedResultAsync(customerId);

            if (result == null)
            {
                return NotFound(
                    new ResponseWrapper<EligibilityEvaluationDto>
                    {
                        Success = false,
                        Message = "No saved eligibility result found.",
                        Data = null
                    });
            }

            return Ok(new ResponseWrapper<EligibilityEvaluationDto>
            {
                Success = true,
                Message = "Saved eligibility result retrieved successfully.",
                Data = result
            });
        }
    }
}
