using LoanApp_API.Application.DTO;
using LoanApp_API.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace LoanApp_API.Application.Interfaces
{
    public interface ISupportTicketRepository
    {
        List<SupportTicket> GetMyTickets(int customerId);

        SupportTicket CreateTicket(SupportTicketDTO model);
        int? GetCustomerIdByUserId(int userId);



    }
}
