using LoanApp_API.Application.DTO.ReportsDTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace LoanApp_API.Application.Interfaces.Reports
{
    public interface IScoreReport
    {
        Task<List<ScoreCardDTO>> fetchScoreCardReport(DateOnly? fromDate, DateOnly? toDate);
    }
}
