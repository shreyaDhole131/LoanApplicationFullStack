using System;
using System.Collections.Generic;
using System.Text;

namespace LoanApp_API.Application.DTO
{
    public class LoanPreviewRequestDTO
    {
        public decimal LoanAmount { get; set; }
        public int TenureMonths { get; set; }
    }
}
