using LoanApp_API.Application.DTO;
using LoanApp_API.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LoanApp_API.Controllers
{
    [ApiController]
    [Route("api/officer/support-tickets")]
    [Authorize(Roles = "2")]
    public class OfficerSupportTicketController : ControllerBase
    {
        private readonly IOfficerSupportTicketRepository _repo;
        private readonly IWebHostEnvironment _environment;

        public OfficerSupportTicketController(
            IOfficerSupportTicketRepository repo,
            IWebHostEnvironment environment)
        {
            _repo = repo;
            _environment = environment;
        }

        // GET: api/officer/support-tickets
        [HttpGet]
        public IActionResult GetAllTickets()
        {
            var tickets = _repo.GetAllTickets();

            return Ok(tickets);
        }

        // GET: api/officer/support-tickets/{ticketId}
        [HttpGet("{ticketId}")]
        public IActionResult GetTicketById(int ticketId)
        {
            var ticket = _repo.GetTicketById(ticketId);

            if (ticket == null)
            {
                return NotFound(new
                {
                    message = "Support ticket not found"
                });
            }

            return Ok(ticket);
        }

        // GET: api/officer/support-tickets/{ticketId}/view
        [HttpGet("{ticketId}/view")]
        public IActionResult ViewAttachment(int ticketId)
        {
            var ticket = _repo.GetTicketById(ticketId);

            if (ticket == null)
            {
                return NotFound(new
                {
                    message = "Support ticket not found"
                });
            }

            if (string.IsNullOrEmpty(ticket.AttachmentPath))
            {
                return NotFound(new
                {
                    message = "Attachment not found"
                });
            }

            var folderPath = Path.Combine(
                _environment.WebRootPath ?? "wwwroot",
                "SupportDocuments"
            );

            var filePath = Path.Combine(
                folderPath,
                ticket.AttachmentPath
            );

            if (!System.IO.File.Exists(filePath))
            {
                return NotFound(new
                {
                    message = "File not found"
                });
            }

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

            return PhysicalFile(
                filePath,
                contentType
            );
        }

        // PUT: api/officer/support-tickets/{ticketId}
        [HttpPut("{ticketId}")]
        public IActionResult UpdateTicket(
            int ticketId,
            OfficerSupportTicketUpdateDTO model)
        {
            if (string.IsNullOrWhiteSpace(model.Status))
            {
                return BadRequest(new
                {
                    message = "Status is required"
                });
            }

            var result =
                _repo.UpdateTicket(ticketId, model);

            if (!result)
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

        // PUT: api/officer/support-tickets/{ticketId}/resolve
        [HttpPut("{ticketId}/resolve")]
        public IActionResult ResolveTicket(int ticketId)
        {
            var model = new OfficerSupportTicketUpdateDTO
            {
                Status = "Resolved",
                OfficerResponse = "Ticket resolved by officer"
            };

            var result =
                _repo.UpdateTicket(ticketId, model);

            if (!result)
            {
                return NotFound(new
                {
                    message = "Support ticket not found"
                });
            }

            return Ok(new
            {
                message = "Support ticket resolved successfully"
            });
        }
    }
}