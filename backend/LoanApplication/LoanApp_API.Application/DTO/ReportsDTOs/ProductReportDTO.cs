using System;
using System.Collections.Generic;
using System.Text;

namespace LoanApp_API.Application.DTO.ReportsDTOs
{
    public class ProductReportDTO
    {
        public string Product { get; set; }

        public int Applicants { get; set; }

        public int Approved { get; set; }

        public int Disbursed { get; set; }

        public decimal DisbursedPercent { get; set; }
    }
}
