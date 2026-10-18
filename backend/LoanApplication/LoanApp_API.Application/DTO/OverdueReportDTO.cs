using System;
using System.Collections.Generic;
using System.Text;

namespace LoanApp_API.Application.DTO
{
    public class OverdueReportDTO
    {
        public string DaysOverdue { get; set; }
        public int Accounts { get; set; }
        public decimal Outstanding { get; set; }
    }
}
