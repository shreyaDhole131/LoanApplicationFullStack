using LoanApp_API.Application.DTO;
using LoanApp_API.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LoanApp_API.Controllers
{
    [ApiController]
    [Route("api/support")]
    [Authorize]
    public class SupportTicketController : ControllerBase
    {
        private readonly ISupportTicketRepository _repo;
        private readonly IWebHostEnvironment _environment;

        public SupportTicketController(
            ISupportTicketRepository repo,
            IWebHostEnvironment environment)
        {
            _repo = repo;
            _environment = environment;
        }

        // =========================================================
        // CREATE SUPPORT TICKET
        // =========================================================

        [HttpPost("create")]
        public IActionResult CreateTicket(
            [FromForm] SupportTicketDTO model)
        {
            // Get logged-in user ID from JWT
            var userIdClaim =
                User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userIdClaim == null)
                return Unauthorized();

            int userId = int.Parse(userIdClaim);

            // Get customer ID using logged-in user ID
            var customerId =
                _repo.GetCustomerIdByUserId(userId);

            if (customerId == null)
            {
                return NotFound(new
                {
                    message = "Customer not found"
                });
            }

            // Always use customer ID from logged-in user
            model.CustomerId = customerId.Value;

            // Validate loan account
            if (model.LoanAccountId <= 0)
            {
                return BadRequest(new
                {
                    message = "Please select loan account"
                });
            }

            // Validate subject
            if (string.IsNullOrWhiteSpace(model.Subject))
            {
                return BadRequest(new
                {
                    message = "Please enter subject"
                });
            }

            // Validate description
            if (string.IsNullOrWhiteSpace(model.Description))
            {
                return BadRequest(new
                {
                    message = "Please enter description"
                });
            }

            // =====================================================
            // FILE UPLOAD
            // =====================================================

            if (model.File != null)
            {
                var allowedExtensions = new[]
                {
                    ".jpg",
                    ".jpeg",
                    ".png",
                    ".pdf"
                };

                var extension =
                    Path.GetExtension(model.File.FileName).ToLower();

                // Validate file extension
                if (!allowedExtensions.Contains(extension))
                {
                    return BadRequest(new
                    {
                        message =
                            "Only JPG, JPEG, PNG and PDF files are allowed"
                    });
                }

                // Create SupportDocuments folder
                var folderPath = Path.Combine(
                    _environment.WebRootPath ?? "wwwroot",
                    "SupportDocuments"
                );

                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }

                // Generate unique file name
                var fileName =
                    Guid.NewGuid().ToString() + extension;

                // Full physical file path
                var filePath =
                    Path.Combine(folderPath, fileName);

                // Save actual file
                using (var stream = new FileStream(
                    filePath,
                    FileMode.Create))
                {
                    model.File.CopyTo(stream);
                }

                // =================================================
                // IMPORTANT
                // Save filename in AttachmentPath BEFORE
                // calling repository CreateTicket()
                // =================================================

                model.AttachmentPath = fileName;

                // File itself should not be mapped to database
                model.File = null;
            }

            // Save ticket in database
            var ticket = _repo.CreateTicket(model);

            return Ok(new
            {
                message = "Support ticket created successfully",
                ticketId = ticket.TicketId,
                status = ticket.Status
            });
        }


        // =========================================================
        // GET MY SUPPORT TICKETS
        // =========================================================

        [HttpGet("my-tickets")]
        public IActionResult GetMyTickets()
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

            var tickets =
                _repo.GetMyTickets(customerId.Value);

            return Ok(tickets);
        }


        // =========================================================
        // GET SINGLE TICKET
        // =========================================================

        [HttpGet("{ticketId}")]
        public IActionResult GetTicket(int ticketId)
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

            var ticket =
                _repo.GetTicketById(
                    ticketId,
                    customerId.Value);

            if (ticket == null)
            {
                return NotFound(new
                {
                    message = "Support ticket not found"
                });
            }

            return Ok(ticket);
        }


        // =========================================================
        // VIEW ATTACHMENT
        // =========================================================

        [HttpGet("{ticketId}/view")]
        public IActionResult ViewAttachment(int ticketId)
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

            // Get ticket belonging to logged-in customer
            var ticket =
                _repo.GetTicketById(
                    ticketId,
                    customerId.Value);

            if (ticket == null)
            {
                return NotFound(new
                {
                    message = "Support ticket not found"
                });
            }

            // Check whether attachment exists in database
            if (string.IsNullOrEmpty(ticket.AttachmentPath))
            {
                return NotFound(new
                {
                    message = "Attachment not found"
                });
            }

            // Get SupportDocuments folder
            var folderPath = Path.Combine(
                _environment.WebRootPath ?? "wwwroot",
                "SupportDocuments"
            );

            // Get actual physical file path
            var filePath =
                Path.Combine(
                    folderPath,
                    ticket.AttachmentPath);

            // Check whether physical file exists
            if (!System.IO.File.Exists(filePath))
            {
                return NotFound(new
                {
                    message = "File not found"
                });
            }

            // Determine file type
            var extension =
                Path.GetExtension(filePath).ToLower();

            var contentType = extension switch
            {
                ".pdf" => "application/pdf",
                ".png" => "image/png",
                ".jpg" => "image/jpeg",
                ".jpeg" => "image/jpeg",
                _ => "application/octet-stream"
            };

            // Return actual file
            return PhysicalFile(
                filePath,
                contentType);
        }


        // =========================================================
        // UPDATE SUPPORT TICKET
        // =========================================================

        [HttpPut("{ticketId}")]
        public IActionResult UpdateTicket(
            int ticketId,
            [FromForm] SupportTicketDTO model)
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

            // Validate loan account
            if (model.LoanAccountId <= 0)
            {
                return BadRequest(new
                {
                    message = "Please select loan account"
                });
            }

            // Validate subject
            if (string.IsNullOrWhiteSpace(model.Subject))
            {
                return BadRequest(new
                {
                    message = "Please enter subject"
                });
            }

            // Validate description
            if (string.IsNullOrWhiteSpace(model.Description))
            {
                return BadRequest(new
                {
                    message = "Please enter description"
                });
            }

            // Get existing ticket
            var ticket =
                _repo.GetTicketById(
                    ticketId,
                    customerId.Value);

            if (ticket == null)
            {
                return NotFound(new
                {
                    message = "Support ticket not found"
                });
            }

            // =====================================================
            // UPDATE ATTACHMENT
            // =====================================================

            if (model.File != null)
            {
                var allowedExtensions = new[]
                {
                    ".jpg",
                    ".jpeg",
                    ".png",
                    ".pdf"
                };

                var extension =
                    Path.GetExtension(model.File.FileName).ToLower();

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
                    "SupportDocuments"
                );

                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }

                // Delete old attachment
                if (!string.IsNullOrEmpty(ticket.AttachmentPath))
                {
                    var oldFilePath =
                        Path.Combine(
                            folderPath,
                            ticket.AttachmentPath);

                    if (System.IO.File.Exists(oldFilePath))
                    {
                        System.IO.File.Delete(oldFilePath);
                    }
                }

                // Generate new filename
                var fileName =
                    Guid.NewGuid().ToString() + extension;

                var filePath =
                    Path.Combine(folderPath, fileName);

                // Save new file
                using (var stream = new FileStream(
                    filePath,
                    FileMode.Create))
                {
                    model.File.CopyTo(stream);
                }

                // Save new filename
                model.AttachmentPath = fileName;

                model.File = null;
            }

            // Use logged-in customer's ID
            model.CustomerId = customerId.Value;

            var updated =
                _repo.UpdateTicket(
                    ticketId,
                    customerId.Value,
                    model);

            if (!updated)
            {
                return NotFound(new
                {
                    message = "Support ticket not found"
                });
            }

            return Ok(new
            {
                message = "Support ticket updated successfully"
            });
        }


        // =========================================================
        // DELETE SUPPORT TICKET
        // =========================================================

        [HttpDelete("{ticketId}")]
        public IActionResult DeleteTicket(int ticketId)
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

            // Get ticket
            var ticket =
                _repo.GetTicketById(
                    ticketId,
                    customerId.Value);

            if (ticket == null)
            {
                return NotFound(new
                {
                    message = "Support ticket not found"
                });
            }

            // =====================================================
            // DELETE PHYSICAL ATTACHMENT
            // =====================================================

            if (!string.IsNullOrEmpty(ticket.AttachmentPath))
            {
                var folderPath = Path.Combine(
                    _environment.WebRootPath ?? "wwwroot",
                    "SupportDocuments"
                );

                var filePath =
                    Path.Combine(
                        folderPath,
                        ticket.AttachmentPath);

                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }
            }

            // Delete ticket from database
            var deleted =
                _repo.DeleteTicket(
                    ticketId,
                    customerId.Value);

            if (!deleted)
            {
                return NotFound(new
                {
                    message = "Support ticket not found"
                });
            }

            return Ok(new
            {
                message = "Support ticket deleted successfully"
            });
        }
    }
}