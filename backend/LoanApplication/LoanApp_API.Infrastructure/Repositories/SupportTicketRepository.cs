using AutoMapper;
using LoanApp_API.Application.DTO;
using LoanApp_API.Application.Interfaces;
using LoanApp_API.Domain.Entities;
using LoanApp_API.Infrastructure.Data;

namespace LoanApp_API.Infrastructure.Repositories
{
    public class SupportTicketRepository : ISupportTicketRepository
    {
        private readonly AppDbContext _db;
        private readonly IMapper _mapper;

        public SupportTicketRepository(
            AppDbContext db,
            IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        public int? GetCustomerIdByUserId(int userId)
        {
            var user = _db.Users
                .FirstOrDefault(x => x.UserId == userId);

            return user?.CustomerId;
        }

        public SupportTicket CreateTicket(SupportTicketDTO model)
        {
            var ticket = _mapper.Map<SupportTicket>(model);

            ticket.Status = "Open";
            ticket.CreatedDate = DateTime.UtcNow;

            _db.SupportTickets.Add(ticket);
            _db.SaveChanges();

            return ticket;
        }

        public List<SupportTicket> GetMyTickets(int customerId)
        {
            return _db.SupportTickets
                .Where(x => x.CustomerId == customerId)
                .OrderByDescending(x => x.CreatedDate)
                .ToList();
        }

        public SupportTicket? GetTicketById(
            int ticketId,
            int customerId)
        {
            return _db.SupportTickets
                .FirstOrDefault(x =>
                    x.TicketId == ticketId &&
                    x.CustomerId == customerId);
        }

        public bool UpdateTicket(
            int ticketId,
            int customerId,
            SupportTicketDTO model)
        {
            var ticket = _db.SupportTickets
                .FirstOrDefault(x =>
                    x.TicketId == ticketId &&
                    x.CustomerId == customerId);

            if (ticket == null)
                return false;

            ticket.LoanAccountId = model.LoanAccountId;
            ticket.Subject = model.Subject;
            ticket.Description = model.Description;

            _db.SaveChanges();

            return true;
        }

        public bool DeleteTicket(
            int ticketId,
            int customerId)
        {
            var ticket = _db.SupportTickets
                .FirstOrDefault(x =>
                    x.TicketId == ticketId &&
                    x.CustomerId == customerId);

            if (ticket == null)
                return false;

            _db.SupportTickets.Remove(ticket);
            _db.SaveChanges();

            return true;
        }
    }
}