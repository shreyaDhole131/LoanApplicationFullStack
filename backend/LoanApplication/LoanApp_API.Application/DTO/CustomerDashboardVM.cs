using System;
using System.Collections.Generic;

namespace LoanApp_API.Application.DTO
{
    public class CustomerDashboardVM
    {
        public int TotalApplications { get; set; }

        public int TotalActiveLoanAccounts { get; set; }

        public List<CustomerLoanDropdownVM> LoanAccounts { get; set; } = new();

        public string? SelectedLoanAccountNo { get; set; }

        public CustomerLoanDetailsVM? SelectedLoanDetails { get; set; }

        public List<CustomerApplicationVM> Applications { get; set; } = new();

        public CustomerEmiSummaryVM EmiSummary { get; set; } = new();

        public List<CustomerEmiPaymentVM> RecentPayments { get; set; } = new();

        public List<CustomerKycVM> KycDocuments { get; set; } = new();
    }

    public class CustomerLoanDropdownVM
    {
        public string LoanAccountNo { get; set; } = string.Empty;
    }

    public class CustomerLoanDetailsVM
    {
        public string LoanAccountNo { get; set; } = string.Empty;

        public string LoanType { get; set; } = string.Empty;

        public decimal LoanAmount { get; set; }

        public decimal OutstandingPrincipal { get; set; }

        public string LoanStatus { get; set; } = string.Empty;

        public decimal InterestRate { get; set; }

        public int TenureMonths { get; set; }

        public decimal EmiAmount { get; set; }

        public DateTime DisbursementDate { get; set; }

        public decimal TotalPaidAmount { get; set; }

        public DateTime? NextEmiDate { get; set; }

        public int TotalEmis { get; set; }

        public int PaidEmis { get; set; }

        public int RemainingEmis { get; set; }
    }

    public class CustomerApplicationVM
    {
        public int DealId { get; set; }

        public string LoanType { get; set; } = string.Empty;

        public decimal LoanAmount { get; set; }

        public decimal ApprovedAmount { get; set; }

        public string CurrentStatus { get; set; } = string.Empty;

        public DateTime AppliedDate { get; set; }
    }

    public class CustomerEmiSummaryVM
    {
        public int TotalEmis { get; set; }

        public int PaidEmis { get; set; }

        public int RemainingEmis { get; set; }

        public decimal TotalPaidAmount { get; set; }

        public DateTime? NextEmiDate { get; set; }

        public decimal? NextEmiAmount { get; set; }
    }

    public class CustomerEmiPaymentVM
    {
        public int InstallmentNo { get; set; }

        public DateTime DueDate { get; set; }

        public decimal Emi { get; set; }

        public string PaymentStatus { get; set; } = string.Empty;

        public DateTime? PaidDate { get; set; }
    }

    public class CustomerKycVM
    {
        public int DocumentId { get; set; }

        public string DocumentType { get; set; } = string.Empty;

        public string VerificationStatus { get; set; } = string.Empty;
    }
}