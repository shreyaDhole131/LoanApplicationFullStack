using LoanApp_API.Application.DTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace LoanApp_API.Application.Interfaces.Reports
{
    public interface IOverdueReport
    {
        Task<List<OverdueReportDTO>> fetchOverdueReport(DateOnly? asOnDate);
    }
}
