using LoanApp_API.Application.DTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace LoanApp_API.Application.Interfaces
{
    public interface IPendingDeals
    {
        Task<List<PendingDealsDTO>> fetchPendingDeals();

        Task<CustomerShowProfileDTO?> fetchCustomerProfile(int dealId);

        Task<List<PendingDealsDTO>> sortPendingDeals(string? LoanType,decimal? MinAmount,decimal? MaxAmount,string? dateFilter, DateOnly? FromFilter,
            DateOnly? ToFilter);

        Task reviewDeal(int dealId,string status);
    }
}
