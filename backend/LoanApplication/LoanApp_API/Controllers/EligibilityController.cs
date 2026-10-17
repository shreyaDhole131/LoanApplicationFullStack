using LoanApp_API.Application.DTO.Common;
using LoanApp_API.Application.DTO.Eligibility;
using LoanApp_API.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

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

        // POST: api/Eligibility/process-automatic
        [Authorize(Roles = "2")]
        [HttpPost("process-automatic")]
        public async Task<ActionResult<ResponseWrapper<int>>>
            ProcessAutomaticEligibility()
        {
            int processed =
                await service.ProcessAutomaticEligibilityAsync();

            return Ok(new ResponseWrapper<int>
            {
                Success = true,
                Message =
                    $"{processed} automatic eligibility decisions processed.",
                Data = processed
            });
        }

        // GET: api/Eligibility/manual-review-customers
        [Authorize(Roles = "2")]
        [HttpGet("manual-review-customers")]
        public async Task<ActionResult<
            ResponseWrapper<List<EligibilityCustomerDto>>>>
            GetManualReviewCustomers()
        {
            var customers =
                await service.GetManualReviewCustomersAsync();

            return Ok(new ResponseWrapper<List<EligibilityCustomerDto>>
            {
                Success = true,
                Message =
                    "Manual Review customers retrieved successfully.",
                Data = customers
            });
        }

        // POST: api/Eligibility/evaluate/12
        [Authorize(Roles = "2")]
        [HttpPost("evaluate/{customerId:int}")]
        public async Task<ActionResult<
            ResponseWrapper<EligibilityEvaluationDto>>>
            Evaluate(int customerId)
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

            var result = await service.EvaluateAsync(customerId);

            if (result == null)
            {
                return NotFound(
                    new ResponseWrapper<EligibilityEvaluationDto>
                    {
                        Success = false,
                        Message =
                            "Pending customer or CIBIL report not found, or the application has already been processed.",
                        Data = null
                    });
            }

            return Ok(new ResponseWrapper<EligibilityEvaluationDto>
            {
                Success = true,
                Message = "Eligibility evaluation completed.",
                Data = result
            });
        }

        // POST: api/Eligibility/approve/12
        [Authorize(Roles = "2")]
        [HttpPost("approve/{customerId:int}")]
        public async Task<ActionResult<
            ResponseWrapper<EligibilityEvaluationDto>>>
            Approve(int customerId)
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

            var result = await service.ApproveAsync(customerId);

            if (result == null)
            {
                return NotFound(
                    new ResponseWrapper<EligibilityEvaluationDto>
                    {
                        Success = false,
                        Message =
                            "Pending application not found or already processed.",
                        Data = null
                    });
            }

            return Ok(new ResponseWrapper<EligibilityEvaluationDto>
            {
                Success = true,
                Message = "Customer eligibility approved successfully.",
                Data = result
            });
        }

        // POST: api/Eligibility/reject/12
        [Authorize(Roles = "2")]
        [HttpPost("reject/{customerId:int}")]
        public async Task<ActionResult<
            ResponseWrapper<EligibilityEvaluationDto>>>
            Reject(
                int customerId,
                [FromBody] EligibilityRejectDto model)
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

            if (model == null)
            {
                return BadRequest(
                    new ResponseWrapper<EligibilityEvaluationDto>
                    {
                        Success = false,
                        Message = "Rejection details are required.",
                        Data = null
                    });
            }

            string reason =
                string.Equals(
                    model.RejectionReason,
                    "Other",
                    StringComparison.OrdinalIgnoreCase)
                    ? model.OtherReason ?? string.Empty
                    : model.RejectionReason;

            if (string.IsNullOrWhiteSpace(reason))
            {
                return BadRequest(
                    new ResponseWrapper<EligibilityEvaluationDto>
                    {
                        Success = false,
                        Message = "Please provide a rejection reason.",
                        Data = null
                    });
            }

            var result = await service.RejectAsync(
                customerId,
                reason.Trim());

            if (result == null)
            {
                return NotFound(
                    new ResponseWrapper<EligibilityEvaluationDto>
                    {
                        Success = false,
                        Message =
                            "Pending application not found or already processed.",
                        Data = null
                    });
            }

            return Ok(new ResponseWrapper<EligibilityEvaluationDto>
            {
                Success = true,
                Message = "Customer eligibility rejected successfully.",
                Data = result
            });
        }

        // GET: api/Eligibility/my-result
        [Authorize(Roles = "1")]
        [HttpGet("my-result")]
        public async Task<ActionResult<
            ResponseWrapper<EligibilityEvaluationDto>>>
            GetMyResult()
        {
            string? userIdValue =
                User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!int.TryParse(userIdValue, out int userId))
            {
                return Unauthorized(
                    new ResponseWrapper<EligibilityEvaluationDto>
                    {
                        Success = false,
                        Message = "Invalid user identity.",
                        Data = null
                    });
            }

            var result =
                await service.GetCustomerResultAsync(userId);

            if (result == null)
            {
                return NotFound(
                    new ResponseWrapper<EligibilityEvaluationDto>
                    {
                        Success = false,
                        Message =
                            "No eligibility result is available for your account.",
                        Data = null
                    });
            }

            return Ok(new ResponseWrapper<EligibilityEvaluationDto>
            {
                Success = true,
                Message = "Eligibility result retrieved successfully.",
                Data = result
            });
        }
    }
}