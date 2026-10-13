using System;
using System.Collections.Generic;
using System.Text;

namespace LoanApp_API.Application.DTO
{
    public class Dashboard
    {
        public int TotalCustomers { get; set; }

        public int TotalLoanCustomers { get; set; }

        public int TotalApprovedKyc { get; set; }

        public decimal TotalLoanAmount { get; set; }

        public int TotalActiveLoans { get; set; }

        public decimal TodaysCollection { get; set; }

        public int PendingEmiCount { get; set; }

        public int ForeClosureRequests { get; set; }
    }
}
