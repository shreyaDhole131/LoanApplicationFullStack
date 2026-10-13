using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace LoanApp_API.Domain.Entities
{
    public class LoanAccount
    {
        [Key]
        public int LoanAccountId { get; set; }

        [ForeignKey("LoanDeal")]
        public int DealId { get; set; }

        [ForeignKey("Customer")]
        public int CustomerId { get; set; }

        public string LoanAccountNo { get; set; }

        public decimal LoanAmount { get; set; }

        public decimal OutstandingPrincipal { get; set; }

        public string LoanStatus { get; set; }

        public decimal InterestRate { get; set; }

        public int TenureMonths { get; set; }

        public decimal EmiAmount { get; set; }

        public DateTime DisbursementDate { get; set; }

        public decimal TotalPaidAmount { get; set; }

        public DateTime CreatedAt { get; set; }

        public Customers? Customer { get; set; }

        public LoanDeals? LoanDeal { get; set; }
        
        public List<EmiSchedule> EmiSchedules { get; set; }
    }
}
