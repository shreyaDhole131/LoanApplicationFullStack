using System;
using System.Collections.Generic;
using System.Text;

namespace LoanApp_API.Application.DTO.ForeClosure
{
    public class ForeClosureDto
    {
        public int CustomerId { get; set; }          // later: take it from the JWT token
        public int LoanAccountId { get; set; }
        public string ForeClosureType { get; set; } = "";   // "Full" or "Partial"
        public decimal PartialAmount { get; set; }          // only for Partial
        public DateTime ExpectedClosureDate { get; set; }
        public string? Reason { get; set; }
    }

    // "Select Loan Account" dropdown
    public class CustomerLoanDto
    {
        public int LoanAccountId { get; set; }
        public string LoanAccountNo { get; set; } = "";
    }

    // 3 cards on top of the customer page
    public class ForeClosureSummaryDto
    {
        public int LoanAccountId { get; set; }
        public string LoanAccountNo { get; set; } = "";
        public decimal OutstandingPrincipal { get; set; }
        public DateTime? NextEmiDate { get; set; }          // null => "No Pending EMI"
    }

    // "My Foreclosure Requests" table
    public class CustomerForeClosureDto
    {
        public string LoanAccountNo { get; set; } = "";
        public decimal Amount { get; set; }
        public string ForeClosureType { get; set; } = "";
        public string Reason { get; set; } = "";
        public string Status { get; set; } = "";
    }

    // ===== OFFICER SIDE =====
    // ONE dto used for both the dashboard table and the View page
    public class OfficerForeClosureDto
    {
        public int RequestId { get; set; }
        public string RequestNo => $"FR{100 + RequestId}";   // FR106 for RequestId 6

        public string LoanAccountNo { get; set; } = "";
        public string CustomerName { get; set; } = "";
        public decimal LoanAmount { get; set; }
        public decimal InterestRate { get; set; }
        public DateTime DisbursedOn { get; set; }
        public decimal OutstandingPrincipal { get; set; }

        public string ForeClosureType { get; set; } = "";
        public DateTime RequestDate { get; set; }
        public DateTime ExpectedClosureDate { get; set; }
        public decimal ForeClosureAmount { get; set; }
        public string Status { get; set; } = "";
        public string CustomerNote { get; set; } = "";
    }
}

