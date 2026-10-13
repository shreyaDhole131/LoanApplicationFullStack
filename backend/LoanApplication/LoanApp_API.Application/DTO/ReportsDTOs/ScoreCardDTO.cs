using System;
using System.Collections.Generic;
using System.Text;

namespace LoanApp_API.Application.DTO.ReportsDTOs
{
    public class ScoreCardDTO
    {
        public string ScoreCard { get; set; }

        public int Applicants { get; set; }

        public int Approved { get; set; }

        public int Rejected { get; set; }

        public int Pending { get; set; }

        public decimal Approval { get; set; }
    }
}
