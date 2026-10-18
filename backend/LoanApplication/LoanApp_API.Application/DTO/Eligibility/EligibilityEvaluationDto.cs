using System.Collections.Generic;

namespace LoanApp_API.Application.DTO.Eligibility
{
    public class EligibilityEvaluationDto
    {
        public int EligibilityId { get; set; }

        public int CustomerId { get; set; }

        public string CustomerName { get; set; } = string.Empty;

        public string PanNumber { get; set; } = string.Empty;

        public int CibilScore { get; set; }

        public string ApplicationStatus { get; set; } = string.Empty;

        public decimal LoanAmount { get; set; }

        public string? RejectionReason { get; set; }

        public decimal MonthlyIncome { get; set; }

        // EMI calculation details
        public decimal ExistingMonthlyEmi { get; set; }

        public decimal MaxAllowableEmi { get; set; }

        public decimal AvailableEmi { get; set; }

        public decimal InterestRate { get; set; }

        // Calculations for 3, 5, 6 and 10-year loan tenures
        public List<TenureEligibilityDto> TenureOptions { get; set; }
            = new List<TenureEligibilityDto>();
    }
}