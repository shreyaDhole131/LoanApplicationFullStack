using LoanApp_API.Application.DTO;
using LoanApp_API.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LoanApp_API.Controllers
{
    [ApiController]
    [Route("api/officer/kyc")]
    [Authorize(Roles = "2")]
    public class OfficerKycController : ControllerBase
    {
        private readonly IOfficerKycRepository _repo;
        private readonly IWebHostEnvironment _environment;

        public OfficerKycController(
            IOfficerKycRepository repo,
            IWebHostEnvironment environment)
        {
            _repo = repo;
            _environment = environment;
        }

        // Get all customers who have uploaded KYC
        [HttpGet]
        public IActionResult GetKycCustomers()
        {
            var customers = _repo.GetKycCustomers();

            return Ok(customers);
        }

        // Get one customer's KYC documents
        [HttpGet("{customerId}")]
        public IActionResult GetCustomerKyc(int customerId)
        {
            var customer = _repo.GetCustomerKyc(customerId);

            if (customer == null)
            {
                return NotFound(new
                {
                    message = "Customer not found"
                });
            }

            return Ok(customer);
        }

        // View actual uploaded KYC document
        [HttpGet("document/{documentId}/view")]
        public IActionResult ViewDocument(int documentId)
        {
            var document = _repo.GetDocumentById(documentId);

            if (document == null)
            {
                return NotFound(new
                {
                    message = "KYC document not found"
                });
            }

            var folderPath = Path.Combine(
                _environment.WebRootPath ?? "wwwroot",
                "KycDocuments"
            );

            var filePath = Path.Combine(
                folderPath,
                document.FilePath
            );

            if (!System.IO.File.Exists(filePath))
            {
                return NotFound(new
                {
                    message = "File not found"
                });
            }

            var extension = Path.GetExtension(filePath).ToLower();

            var contentType = extension switch
            {
                ".pdf" => "application/pdf",
                ".png" => "image/png",
                ".jpg" => "image/jpeg",
                ".jpeg" => "image/jpeg",
                _ => "application/octet-stream"
            };

            return PhysicalFile(
                filePath,
                contentType
            );
        }

        // Approve KYC documents
        [HttpPut("approve")]
        public IActionResult ApproveKyc(OfficerKycStatusDTO model)
        {
            if (model.DocumentIds == null ||
                model.DocumentIds.Count == 0)
            {
                return BadRequest(new
                {
                    message = "Please select at least one document"
                });
            }

            var result = _repo.UpdateKycStatus(
                model.DocumentIds,
                "Approved"
            );

            if (!result)
            {
                return NotFound(new
                {
                    message = "KYC documents not found"
                });
            }

            return Ok(new
            {
                message = "KYC approved successfully"
            });
        }

        // Reject KYC documents
        [HttpPut("reject")]
        public IActionResult RejectKyc(OfficerKycStatusDTO model)
        {
            if (model.DocumentIds == null ||
                model.DocumentIds.Count == 0)
            {
                return BadRequest(new
                {
                    message = "Please select at least one document"
                });
            }

            var result = _repo.UpdateKycStatus(
                model.DocumentIds,
                "Rejected"
            );

            if (!result)
            {
                return NotFound(new
                {
                    message = "KYC documents not found"
                });
            }

            return Ok(new
            {
                message = "KYC rejected successfully"
            });
        }
    }
}