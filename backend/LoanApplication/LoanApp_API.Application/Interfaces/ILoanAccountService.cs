using LoanApp_API.Application.DTO;
using LoanApp_API.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace LoanApp_API.Application.Interfaces
{
    public interface ILoanAccountService
    {
        Task CreateLoanAccount(int disbursementId);
        Task<List<LoanAccountDto>> FetchAllLoanAccounts();

        Task<List<LoanAccountDto>> FetchLoanAccountById(int customerId);

    }
}
