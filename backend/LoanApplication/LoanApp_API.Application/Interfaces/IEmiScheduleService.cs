using LoanApp_API.Application.DTO;
using LoanApp_API.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace LoanApp_API.Application.Interfaces
{
    public interface IEmiScheduleService
    {
        Task GenerateEMISchedule(int loanAccountId);
        Task<EmiScheduleSummaryDto> GetEMISchedule(int loanAccountId);
    }
}
