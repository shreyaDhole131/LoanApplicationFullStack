using System;
using System.Collections.Generic;
using System.Text;

namespace LoanApp_API.Application.DTO.ReportsDTOs
{
    public class CibiReportDTO
    {
        public string CibilScoreRange { get; set; }

        public int Applicants { get; set; }

        public int Approved { get; set; }

        public decimal Approval { get; set; }


    }
}
