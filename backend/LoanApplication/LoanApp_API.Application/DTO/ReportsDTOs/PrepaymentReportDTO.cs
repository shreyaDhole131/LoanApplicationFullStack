using System;
using System.Collections.Generic;
using System.Text;

namespace LoanApp_API.Application.DTO.ReportsDTOs
{
    public class PrepaymentReportDTO
    {
        public string Type { get; set; }

        public int  Accounts { get; set; }

        public decimal PrepaymentAmount { get; set; }
    }
}
