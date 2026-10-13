using System;
using System.Collections.Generic;
using System.Text;

namespace LoanApp_API.Application.DTO.ReportsDTOs
{
    public class EmiReportDTO
    {
        public string Particulars { get; set; }

        public int Count { get; set; }

        public decimal Amount { get; set; }

        public decimal Percent { get; set; }
    }
}
