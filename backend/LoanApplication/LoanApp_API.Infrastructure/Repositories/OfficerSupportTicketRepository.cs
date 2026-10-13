using LoanApp_API.Application.DTO;
using LoanApp_API.Application.Interfaces;
using LoanApp_API.Infrastructure.Data;

namespace LoanApp_API.Infrastructure.Repositories
{
    public class OfficerSupportTicketRepository : IOfficerSupportTicketRepository
    {
        private readonly AppDbContext _db;

        public OfficerSupportTicketRepository(AppDbContext db)
        {
            _db = db;
        }

        public List<OfficerSupportTicketVM> GetAllTickets()
        {
            return _db.SupportTickets
                .Select(ticket => new OfficerSupportTicketVM
                {
                    TicketId = ticket.TicketId,

                    CustomerId = ticket.CustomerId,

                    CustomerName = ticket.Customer != null
                        ? ticket.Customer.FirstName + " " + ticket.Customer.LastName
                        : string.Empty,

                    Email = ticket.Customer != null
                        ? ticket.Customer.Email
                        : string.Empty,

                    LoanAccountId = ticket.LoanAccountId,

                    LoanAccountNo = ticket.LoanAccount != null
                        ? ticket.LoanAccount.LoanAccountNo
                        : string.Empty,

                    Subject = ticket.Subject,

                    Description = ticket.Description,

                    AttachmentPath = ticket.AttachmentPath,

                    CreatedDate = ticket.CreatedDate,

                    Status = ticket.Status,

                    OfficerResponse = ticket.OfficerResponse
                })
                .ToList();
        }

        public OfficerSupportTicketVM? GetTicketById(int ticketId)
        {
            return _db.SupportTickets
                .Where(ticket => ticket.TicketId == ticketId)
                .Select(ticket => new OfficerSupportTicketVM
                {
                    TicketId = ticket.TicketId,

                    CustomerId = ticket.CustomerId,

                    CustomerName = ticket.Customer != null
                        ? ticket.Customer.FirstName + " " + ticket.Customer.LastName
                        : string.Empty,

                    Email = ticket.Customer != null
                        ? ticket.Customer.Email
                        : string.Empty,

                    LoanAccountId = ticket.LoanAccountId,

                    LoanAccountNo = ticket.LoanAccount != null
                        ? ticket.LoanAccount.LoanAccountNo
                        : string.Empty,

                    Subject = ticket.Subject,

                    Description = ticket.Description,

                    AttachmentPath = ticket.AttachmentPath,

                    CreatedDate = ticket.CreatedDate,

                    Status = ticket.Status,

                    OfficerResponse = ticket.OfficerResponse
                })
                .FirstOrDefault();
        }

        public bool UpdateTicket(
            int ticketId,
            OfficerSupportTicketUpdateDTO model)
        {
            var ticket = _db.SupportTickets
                .FirstOrDefault(x => x.TicketId == ticketId);

            if (ticket == null)
                return false;

            ticket.Status = model.Status;
            ticket.OfficerResponse = model.OfficerResponse;

            _db.SaveChanges();

            return true;
        }
    }
}