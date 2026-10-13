using LoanApp_API.Application.DTO;
using LoanApp_API.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LoanApp_API.Controllers
{
    [ApiController]
    [Route("api/support-ticket")]
    [Authorize]
    public class SupportTicketController : ControllerBase
    {
        private readonly ISupportTicketRepository _repo;

        public SupportTicketController(ISupportTicketRepository repo)
        {
            _repo = repo;
        }

        [HttpPost("create")]
        public IActionResult CreateTicket(SupportTicketDTO model)
        {
            var ticket = _repo.CreateTicket(model);

            return Ok(new
            {
                message = "Support ticket created successfully",
                ticketId = ticket.TicketId
            });
        }
        [HttpGet("my-tickets")]
        public IActionResult GetMyTickets()
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

            var tickets = _repo.GetMyTickets(customerId.Value);

            return Ok(tickets);
        }
    }
}