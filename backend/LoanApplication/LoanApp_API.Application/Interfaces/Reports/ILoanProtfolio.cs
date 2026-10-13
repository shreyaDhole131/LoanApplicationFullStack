using LoanApp_API.Application.DTO.ReportsDTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace LoanApp_API.Application.Interfaces.Reports
{
    public interface ILoanProtfolio
    {
        Task<List<LoanProtfolioDTO>> fetchLoanProtofolio(DateOnly? fromDate ,
    DateOnly? toDate );
    }
}
