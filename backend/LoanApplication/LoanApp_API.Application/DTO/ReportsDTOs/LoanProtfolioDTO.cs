using System;
using System.Collections.Generic;
using System.Text;

namespace LoanApp_API.Application.DTO.ReportsDTOs
{
    public class LoanProtfolioDTO
    {
        public string Product { get; set; }

        public int AciveLoans { get; set; }

        public decimal Outstanding { get; set; }

        public decimal Percent { get; set; }
    }
}
