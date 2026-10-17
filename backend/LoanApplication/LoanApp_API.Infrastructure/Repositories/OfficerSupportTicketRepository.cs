using AutoMapper;
using LoanApp_API.Application.DTO;
using LoanApp_API.Application.Interfaces;
using LoanApp_API.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LoanApp_API.Infrastructure.Repositories
{
    public class OfficerSupportTicketRepository
        : IOfficerSupportTicketRepository
    {
        private readonly AppDbContext _db;
        private readonly IMapper _mapper;

        public OfficerSupportTicketRepository(
            AppDbContext db,
            IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        // Get all customer support tickets
        public List<OfficerSupportTicketVM> GetAllTickets()
        {
            var tickets = _db.SupportTickets
                .Include(x => x.Customer)
                .Include(x => x.LoanAccount)
                .ToList();

            return _mapper.Map<List<OfficerSupportTicketVM>>(tickets);
        }

        // Get particular support ticket
        public OfficerSupportTicketVM? GetTicketById(
            int ticketId)
        {
            var ticket = _db.SupportTickets
                .Include(x => x.Customer)
                .Include(x => x.LoanAccount)
                .FirstOrDefault(x =>
                    x.TicketId == ticketId);

            if (ticket == null)
            {
                return null;
            }

            return _mapper.Map<OfficerSupportTicketVM>(ticket);
        }

        // Update ticket status and officer response
        public bool UpdateTicket(
            int ticketId,
            OfficerSupportTicketUpdateDTO model)
        {
            var ticket = _db.SupportTickets
                .FirstOrDefault(x =>
                    x.TicketId == ticketId);

            if (ticket == null)
            {
                return false;
            }

            ticket.Status = model.Status;
            ticket.OfficerResponse = model.OfficerResponse;

            _db.SaveChanges();

            return true;
        }
    }
}