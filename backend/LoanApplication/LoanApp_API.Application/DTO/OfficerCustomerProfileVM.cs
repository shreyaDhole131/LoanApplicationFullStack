using System;
using System.Collections.Generic;

namespace LoanApp_API.Application.DTO
{
    public class OfficerCustomerProfileVM
    {
        public int CustomerId { get; set; }

        // Customer Details
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Mobile { get; set; } = string.Empty;
        public string Pan { get; set; } = string.Empty;
        public string Aadhaar { get; set; } = string.Empty;
        public string Salary { get; set; } = string.Empty;
        public string Employment { get; set; } = string.Empty;
        public int Age { get; set; }

        // CIBIL Details
        public int CibilScore { get; set; }

        // Score Card Details
        public string RiskCategory { get; set; } = string.Empty;
        public decimal EligibleLoanAmount { get; set; }

        // Loan Details
        public List<OfficerLoanDetailsVM> Loans { get; set; } = new();
    }

    public class OfficerLoanDetailsVM
    {
        public string LoanAccountNo { get; set; } = string.Empty;
        public decimal LoanAmount { get; set; }
        public decimal EmiAmount { get; set; }
        public decimal InterestRate { get; set; }
        public string LoanStatus { get; set; } = string.Empty;

        public List<OfficerEmiHistoryVM> EmiHistory { get; set; } = new();
    }

    public class OfficerEmiHistoryVM
    {
        public int InstallmentNo { get; set; }
        public DateTime DueDate { get; set; }
        public decimal Emi { get; set; }
        public string PaymentStatus { get; set; } = string.Empty;
        public DateTime? PaidDate { get; set; }
    }
}