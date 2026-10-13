using System;
using System.Collections.Generic;
using System.Text;

namespace LoanApp_API.Application.DTO.ReportsDTOs
{
    public class DisbursementReportDTO
    {
        public string Month { get; set; }

        public int LoanDisbursed { get; set; }

        public decimal DecimalAmount { get; set; }
    }
}
