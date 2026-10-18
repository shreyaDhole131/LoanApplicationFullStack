using System;
using System.Collections.Generic;
using System.Text;

namespace LoanApp_API.Application.DTO.ForeClosure
{
    public class ForeClosureDto
    {
        public int CustomerId { get; set; }          
        public int LoanAccountId { get; set; }
        public string ForeClosureType { get; set; } = "";   
        public decimal PartialAmount { get; set; }          
        public DateTime ExpectedClosureDate { get; set; }
        public string? Reason { get; set; }
    }

    public class CustomerLoanDto
    {
        public int LoanAccountId { get; set; }
        public string LoanAccountNo { get; set; } = "";
    }

    public class ForeClosureSummaryDto
    {
        public int LoanAccountId { get; set; }
        public string LoanAccountNo { get; set; } = "";
        public decimal OutstandingPrincipal { get; set; }
        public DateTime? NextEmiDate { get; set; }          
    }


    public class CustomerForeClosureDto
    {
        public int RequestId { get; set; }

        public string LoanAccountNo { get; set; } = "";
        public decimal Amount { get; set; }
        public string ForeClosureType { get; set; } = "";
        public string Reason { get; set; } = "";
        public string Status { get; set; } = "";
    }


   
    public class OfficerForeClosureDto
    {
        public int RequestId { get; set; }
        public string RequestNo => $"FR{100 + RequestId}";   

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

