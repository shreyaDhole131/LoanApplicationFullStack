using LoanApp_API.Application.DTO.ReportsDTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace LoanApp_API.Application.Interfaces.Reports
{
    public interface ICibilReport
    {
        Task<List<CibiReportDTO>> fetchCibilReport(DateOnly? fromDate, DateOnly? toDate);

    }
}
