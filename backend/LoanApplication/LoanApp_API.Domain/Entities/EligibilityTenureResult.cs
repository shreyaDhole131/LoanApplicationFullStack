
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LoanApp_API.Domain.Entities
{
    public class EligibilityTenureResult
    {
        [Key]
        public int EligibilityTenureResultId { get; set; }

        public int EligibilityId { get; set; }

        public decimal InterestRate { get; set; }

        public int TenureMonths { get; set; }

        public decimal ExistingMonthlyEmi { get; set; }

        public decimal MaxAllowableEmi { get; set; }

        public decimal AvailableEmi { get; set; }

        public decimal EligibleLoanAmount { get; set; }

        public DateTime CalculatedAt { get; set; }

        [ForeignKey(nameof(EligibilityId))]
        public EligibilityResult? EligibilityResult { get; set; }
    }
}
