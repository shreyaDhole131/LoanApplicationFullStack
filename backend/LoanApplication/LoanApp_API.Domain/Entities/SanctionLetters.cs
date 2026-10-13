using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace LoanApp_API.Domain.Entities
{
    public class SanctionLetters
    {
        [Key]
        public int SanctionId { get; set; }

        [ForeignKey("DealId")]
        public int DealId { get; set; }

        public LoanDeals deals { get; set; }

        public decimal LoanAmount { get; set; }

        public int InterestRate { get; set; }

        public int TenureMonths { get; set; }

        public decimal EmiAmount { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
