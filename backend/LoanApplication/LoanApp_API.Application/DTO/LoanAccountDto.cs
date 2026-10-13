using System;
using System.Collections.Generic;
using System.Text;

namespace LoanApp_API.Application.DTO
{
    public class LoanAccountDto
    {
        public int LoanAccountId { get; set; }
        public string LoanAccountNo { get; set; }
        public string CustomerName { get; set; }
        public decimal LoanAmount { get; set; }
        public decimal OutstandingPrincipal { get; set; }
        public string LoanStatus { get; set; }
        public decimal InterestRate { get; set; }
        public int TenureMonths { get; set; }
        public decimal EmiAmount { get; set; }
        public DateTime DisbursementDate { get; set; }
        public decimal TotalPaidAmount { get; set; }
        public DateTime CreatedAt { get; set; }

    }
}
