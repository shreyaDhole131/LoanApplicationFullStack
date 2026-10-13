using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace LoanApp_API.Domain.Entities
{
    public class EligibilityResult
    {
        [Key]
        public int EligibilityId { get; set; }

        [ForeignKey("Customer")]
        public int CustomerId { get; set; }
        public int CibilScore { get; set; }
        public string? IsEligible { get; set; } = "Pending";    
        public decimal LoanAmount { get; set; }
        public string? RejectionReason { get; set; }
        public Customers? Customer { get; set; }
    }
}
