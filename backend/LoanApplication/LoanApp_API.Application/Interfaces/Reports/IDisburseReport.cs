using LoanApp_API.Application.DTO.ReportsDTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace LoanApp_API.Application.Interfaces.Reports
{
    public interface IDisburseReport
    {
        Task<List<DisbursementReportDTO>> fetchDisbursmentReport(DateOnly? fromDate, DateOnly? toDate);
    }
}
