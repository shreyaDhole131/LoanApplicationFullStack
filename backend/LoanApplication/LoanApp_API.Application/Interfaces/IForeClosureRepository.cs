using LoanApp_API.Application.DTO.ForeClosure;
using System;
using System.Collections.Generic;
using System.Text;

namespace LoanApp_API.Application.Interfaces
{
    public interface IForeClosureRepository
    {
        //--- customer side
        Task<List<CustomerLoanDto>> GetCustomerLoans(int customerId);
        Task<ForeClosureSummaryDto?> GetSummary(int loanAccountId);
        Task<(bool Success, string Message)> SubmitRequest(ForeClosureDto dto);
        Task<List<CustomerForeClosureDto>> GetMyRequests(int customerId);

        // - officer side
        Task<List<OfficerForeClosureDto>> GetAllRequests();
        Task<OfficerForeClosureDto?> GetRequestById(int id);
        Task<(bool Success, string Message)> Approve(int id);
        Task<(bool Success, string Message)> Reject(int id);
    }
}
