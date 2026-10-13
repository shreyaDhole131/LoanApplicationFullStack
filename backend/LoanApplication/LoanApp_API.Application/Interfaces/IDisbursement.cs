using LoanApp_API.Application.DTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace LoanApp_API.Application.Interfaces
{
    public interface IDisbursement
    {
        Task<DisbursePrefillDTO?> fetchDisbursePrefill(int dealId);

        Task ApplyDisbursement(CreateDisbursementDTO dto);
    }
}
