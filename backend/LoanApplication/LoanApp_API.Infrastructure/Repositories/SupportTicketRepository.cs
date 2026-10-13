using LoanApp_API.Application.DTO;
using LoanApp_API.Application.Interfaces;
using LoanApp_API.Domain.Entities;
using LoanApp_API.Infrastructure.Data;

namespace LoanApp_API.Infrastructure.Repositories
{
    public class SupportTicketRepository : ISupportTicketRepository
    {
        private readonly AppDbContext _db;

        public SupportTicketRepository(AppDbContext db)
        {
            _db = db;
        }

        public SupportTicket CreateTicket(SupportTicketDTO model)
        {
            var ticket = new SupportTicket
            {
                CustomerId = model.CustomerId,
                LoanAccountId = model.LoanAccountId,
                Subject = model.Subject,
                Description = model.Description,
                AttachmentPath = model.AttachmentPath,
                CreatedDate = DateTime.Now,
                Status = "Open"
            };

            _db.SupportTickets.Add(ticket);
            _db.SaveChanges();
            return ticket;
        }

        public int? GetCustomerIdByUserId(int userId)
        {
            var user = _db.Users.FirstOrDefault(x => x.UserId == userId);
            return user?.CustomerId;
        }

        public List<SupportTicket> GetMyTickets(int customerId)
        {
            return _db.SupportTickets.Where(x => x.CustomerId == customerId).ToList();
        }
    }
}