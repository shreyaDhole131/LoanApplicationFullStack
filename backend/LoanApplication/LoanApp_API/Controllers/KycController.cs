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
        private readonly IWebHostEnvironment _environment;

        public KycController(
            IKycRepository repo,
            IWebHostEnvironment environment)
        {
            _repo = repo;
            _environment = environment;
        }

        // ============================
        // UPLOAD KYC
        // ============================

        [HttpPost("upload")]
        public IActionResult UploadKyc(
            [FromForm] KycDocumentDTO model)
        {
            var userIdClaim =
                User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userIdClaim == null)
                return Unauthorized();

            int userId = int.Parse(userIdClaim);

            var customerId =
                _repo.GetCustomerIdByUserId(userId);

            if (customerId == null)
            {
                return NotFound(new
                {
                    message = "Customer not found"
                });
            }

            if (model.File == null || model.File.Length == 0)
            {
                return BadRequest(new
                {
                    message = "Please select a file"
                });
            }

            if (string.IsNullOrWhiteSpace(model.DocumentType))
            {
                return BadRequest(new
                {
                    message = "Please select document type"
                });
            }

            var allowedExtensions = new[]
            {
                ".jpg",
                ".jpeg",
                ".png",
                ".pdf"
            };

            var extension =
                Path.GetExtension(
                    model.File.FileName
                ).ToLower();

            if (!allowedExtensions.Contains(extension))
            {
                return BadRequest(new
                {
                    message =
                        "Only JPG, JPEG, PNG and PDF files are allowed"
                });
            }

            var folderPath = Path.Combine(
                _environment.WebRootPath ?? "wwwroot",
                "KycDocuments"
            );

            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            var fileName =
                Guid.NewGuid().ToString() + extension;

            var filePath =
                Path.Combine(
                    folderPath,
                    fileName
                );

            using (var stream = new FileStream(
                filePath,
                FileMode.Create))
            {
                model.File.CopyTo(stream);
            }

            // Create DTO for database
            var kycDto = new KycDocumentDTO
            {
                CustomerId = customerId.Value,
                DocumentType = model.DocumentType,
                FilePath = fileName
            };

            var kyc =
                _repo.UploadKyc(kycDto);

            return Ok(new
            {
                message =
                    "KYC document uploaded successfully",

                documentId =
                    kyc.DocumentId,

                verificationStatus =
                    kyc.VerificationStatus
            });
        }

        // ============================
        // GET MY KYC DOCUMENTS
        // ============================

        [HttpGet("my-documents")]
        public IActionResult GetMyDocuments()
        {
            var userIdClaim =
                User.FindFirst(
                    ClaimTypes.NameIdentifier
                )?.Value;

            if (userIdClaim == null)
                return Unauthorized();

            int userId =
                int.Parse(userIdClaim);

            var customerId =
                _repo.GetCustomerIdByUserId(userId);

            if (customerId == null)
            {
                return NotFound(new
                {
                    message =
                        "Customer not found"
                });
            }

            var documents =
                _repo.GetMyKycDocuments(
                    customerId.Value
                );

            return Ok(documents);
        }

        // ============================
        // VIEW MY KYC DOCUMENT
        // ============================

        [HttpGet("{documentId}/view")]
        public IActionResult ViewDocument(
            int documentId)
        {
            var userIdClaim =
                User.FindFirst(
                    ClaimTypes.NameIdentifier
                )?.Value;

            if (userIdClaim == null)
                return Unauthorized();

            int userId =
                int.Parse(userIdClaim);

            var customerId =
                _repo.GetCustomerIdByUserId(
                    userId
                );

            if (customerId == null)
                return NotFound();

            var document =
                _repo.GetKycById(
                    documentId,
                    customerId.Value
                );

            if (document == null)
            {
                return NotFound(new
                {
                    message =
                        "KYC document not found"
                });
            }

            var folderPath =
                Path.Combine(
                    _environment.WebRootPath ?? "wwwroot",
                    "KycDocuments"
                );

            var filePath =
                Path.Combine(
                    folderPath,
                    document.FilePath
                );

            if (!System.IO.File.Exists(filePath))
            {
                return NotFound(new
                {
                    message =
                        "File not found"
                });
            }

            var extension =
                Path.GetExtension(
                    filePath
                ).ToLower();

            var contentType =
                extension switch
                {
                    ".pdf" =>
                        "application/pdf",

                    ".png" =>
                        "image/png",

                    ".jpg" =>
                        "image/jpeg",

                    ".jpeg" =>
                        "image/jpeg",

                    _ =>
                        "application/octet-stream"
                };

            return PhysicalFile(
                filePath,
                contentType
            );
        }

        // ============================
        // DELETE KYC
        // ============================

        [HttpDelete("{documentId}")]
        public IActionResult DeleteKyc(
            int documentId)
        {
            var userIdClaim =
                User.FindFirst(
                    ClaimTypes.NameIdentifier
                )?.Value;

            if (userIdClaim == null)
                return Unauthorized();

            int userId =
                int.Parse(userIdClaim);

            var customerId =
                _repo.GetCustomerIdByUserId(
                    userId
                );

            if (customerId == null)
            {
                return NotFound(new
                {
                    message =
                        "Customer not found"
                });
            }

            var document =
                _repo.GetKycById(
                    documentId,
                    customerId.Value
                );

            if (document == null)
            {
                return NotFound(new
                {
                    message =
                        "KYC document not found"
                });
            }

            var folderPath =
                Path.Combine(
                    _environment.WebRootPath ?? "wwwroot",
                    "KycDocuments"
                );

            var filePath =
                Path.Combine(
                    folderPath,
                    document.FilePath
                );

            // Delete physical file
            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }

            // Delete database record
            var deleted =
                _repo.DeleteKyc(
                    documentId,
                    customerId.Value
                );

            if (!deleted)
            {
                return NotFound(new
                {
                    message =
                        "KYC document not found"
                });
            }

            return Ok(new
            {
                message =
                    "KYC document deleted successfully"
            });
        }
    }
}