using System;
using System.Collections.Generic;
using System.Text;

namespace LoanApp_API.Application.DTO
{
    public class CustomerShowProfileDTO
    {
        public int DealId { get; set; }
        public string CustomerName { get; set; }

        public string MobileNo { get; set; }

        public string Pan { get; set; }

        public string AadhaarNo { get; set; }

        public string MonthlyIncome { get; set; }

        public string LoanType { get; set; }

        public decimal LoanAmount { get; set; }

        public int InterestRate { get; set; }

        public int TenureMonths { get; set; }

        public decimal EmiAmount { get; set; }

        public int CibilScore { get; set; }
    }
}
