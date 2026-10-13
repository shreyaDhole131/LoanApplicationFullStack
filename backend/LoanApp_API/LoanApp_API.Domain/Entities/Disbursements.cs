using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace LoanApp_API.Domain.Entities
{
    public class Disbursements
    {
        [Key]
        public int DisbursementId { get; set; }

        [ForeignKey("DealId")]
        public int DealId { get; set; }

        public LoanDeals deals { get; set; }

        public decimal DisburseAmount { get; set; }

        public string BankPartner { get; set; }

        public DateTime DisbursementDate { get; set; }

        public string Status { get; set; }
    }
}
