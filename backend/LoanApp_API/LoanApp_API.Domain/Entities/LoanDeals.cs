using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace LoanApp_API.Domain.Entities
{
    public class LoanDeals
    {
        [Key]
        public int DealId { get; set; }

        [ForeignKey("CustomerId")]
        public int CustomerId { get; set; }

        public Customers customers { get; set; }

        public string LoanType { get; set; }

        public decimal LoanAmount { get; set; }

        public int InterestRate { get; set; }

        public int TenureMonths { get; set; }

        public decimal EmiAmount { get; set; }

        public string BankName { get; set; }

        public string BankAccountNumber { get; set; }

        public string IFSCCode { get; set; }

        public int EmiDay { get; set; }

        public decimal ApprovedAmount { get; set; }

        public string CurrentStatus { get; set; }

        public DateTime AppliedDate { get; set; }

        public Disbursements disbursement { get; set; }

        public SanctionLetters sanctionLetters { get; set; }

        public DealReviews dealReviews { get; set; }

        public List<LoanAccount> LoanAccounts { get; set; }
        public List<EmiSchedule> EmiSchedules { get; set; }


    }
}
