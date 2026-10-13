using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace LoanApp_API.Domain.Entities
{
    public class ScoreCard
    {
        [Key]
        public int ScoreCardId { get; set; }

        [ForeignKey("Customer")]
        public int CustomerId { get; set; }
        public string RiskCategory { get; set; }
        public decimal EligibleLoanAmount { get; set; }
        public Customers? Customer { get; set; }
    }
}
