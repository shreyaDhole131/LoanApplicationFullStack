using System;
using System.Collections.Generic;
using System.Text;

namespace LoanApp_API.Application.DTO
{
    public class SanctionLetterDTO
    {

        public int SanctionLetterId { get; set; }
        public int DealId { get; set; }
        public decimal LoanAmount { get; set; }
        public string LoanType { get; set; }
        public int TenureMonths { get; set; }
        public decimal InterestRate { get; set; }
        public decimal EmiAmount { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CustomerName { get; set; }
        public string MobileNo { get; set; }

    }
}
