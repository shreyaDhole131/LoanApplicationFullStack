using LoanApp_API.Application.DTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace LoanApp_API.Application.Interfaces.Reports
{
    public interface ITatReport
    {
        Task<List<TatReportDTO>> fetchTatReport(DateOnly? fromDate, DateOnly? toDate);
    }
}
