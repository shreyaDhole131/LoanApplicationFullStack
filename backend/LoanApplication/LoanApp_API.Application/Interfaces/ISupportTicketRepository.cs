using LoanApp_API.Application.DTO;
using LoanApp_API.Domain.Entities;

namespace LoanApp_API.Application.Interfaces
{
    public interface ISupportTicketRepository
    {
        SupportTicket CreateTicket(SupportTicketDTO model);
        List<SupportTicket> GetMyTickets(int customerId);
        SupportTicket? GetTicketById(int ticketId, int customerId);
        bool UpdateTicket(int ticketId, int customerId, SupportTicketDTO model);
        bool DeleteTicket(int ticketId, int customerId);
        int? GetCustomerIdByUserId(int userId);
    }
}