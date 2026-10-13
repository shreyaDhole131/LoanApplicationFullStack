using LoanApp_API.Application.DTO.ForeClosure;
using LoanApp_API.Application.DTO.PaymentHistory;
using System;
using System.Collections.Generic;
using System.Text;

namespace LoanApp_API.Application.Interfaces
{
    public interface IPaymentHistoryRepository
    {
        // get all loans of the customer
        Task<List<CustomerLoanDto>> GetCustomerLoans(int customerId);

        Task<List<PaymentHistoryDto>> GetPaymentHistory(int customerId, int? loanAccountId, int months);
    }
}
