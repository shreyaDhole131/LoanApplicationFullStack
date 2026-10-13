using LoanApp_API.Application.DTO;

namespace LoanApp_API.Application.Interfaces
{
    public interface IOfficerSupportTicketRepository
    {
        List<OfficerSupportTicketVM> GetAllTickets();

        OfficerSupportTicketVM? GetTicketById(int ticketId);

        bool UpdateTicket(
            int ticketId,
            OfficerSupportTicketUpdateDTO model);
    }
}