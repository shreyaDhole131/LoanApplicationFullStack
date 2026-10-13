using LoanApp_API.Application.DTO;
using LoanApp_API.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LoanApp_API.Controllers
{
    [ApiController]
    [Route("api/kyc")]
    [Authorize]
    public class KycController : ControllerBase
    {
        private readonly IKycRepository _repo;

        public KycController(IKycRepository repo)
        {
            _repo = repo;
        }

        [HttpPost("upload")]
        public IActionResult UploadKyc(KycDocumentDTO model)
        {
            var kyc = _repo.UploadKyc(model);

            return Ok(new
            {
                message = "KYC document uploaded successfully",
                documentId = kyc.DocumentId,
                verificationStatus = kyc.VerificationStatus
            });
        }
        [HttpGet("my-documents")]
        public IActionResult GetMyDocuments()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userIdClaim == null)
                return Unauthorized();

            int userId = int.Parse(userIdClaim);

            var customerId = _repo.GetCustomerIdByUserId(userId);

            if (customerId == null)
                return NotFound(new
                {
                    message = "Customer not found"
                });

            var documents = _repo.GetMyKycDocuments(customerId.Value);

            return Ok(documents);
        }
    }
}