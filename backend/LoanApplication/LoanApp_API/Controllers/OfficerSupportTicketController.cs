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

        public OfficerSupportTicketController(
            IOfficerSupportTicketRepository repo)
        {
            _repo = repo;
        }

        [HttpGet]
        public IActionResult GetAllTickets()
        {
            var tickets = _repo.GetAllTickets();

            return Ok(tickets);
        }

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

            var result = _repo.UpdateTicket(ticketId, model);

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
    }
}