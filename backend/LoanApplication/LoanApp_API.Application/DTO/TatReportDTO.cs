using System;
using System.Collections.Generic;
using System.Text;

namespace LoanApp_API.Application.DTO
{
    public class TatReportDTO
    {
        public string Stage { get; set; }
        public decimal AvgDays { get; set; }
        public int MinDays { get; set; }
        public int MaxDays { get; set; }
    }
}
