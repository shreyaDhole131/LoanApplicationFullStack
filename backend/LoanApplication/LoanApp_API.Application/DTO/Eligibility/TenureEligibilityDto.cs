
using System;


using System;

namespace LoanApp_API.Application.DTO.Eligibility
{
    public class TenureEligibilityDto
    {
        public int EligibilityTenureResultId { get; set; }

        public int TenureMonths { get; set; }

        public decimal InterestRate { get; set; }

        public decimal ExistingMonthlyEmi { get; set; }

        public decimal MaxAllowableEmi { get; set; }

        public decimal AvailableEmi { get; set; }

        public decimal EligibleLoanAmount { get; set; }

        public DateTime CalculatedAt { get; set; }
    }
}
