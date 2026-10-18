using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace LoanApp_API.Domain.Entities
{
    public class ForeClosureRequest
    {
        [Key]
        public int RequestId { get; set; }

        [ForeignKey("LoanAccount")]
        public int LoanAccountId { get; set; }


        [Required]
        public string ForeClosureType { get; set; }

        public decimal ForeClosureAmount { get; set; }

        public decimal PartialAmount { get; set; }

        public DateTime RequestedDate { get; set; }

        [Required]
        public DateTime ExpectedClosureDate { get; set; }

        [Required]
        public string Reason { get; set; }

        public string Status { get; set; }

        public DateTime? ApprovedDate { get; set; }

        public bool IsPaid { get; set; } = false;

        public DateTime? PaidDate { get; set; }

        [ForeignKey("User")]
        public int? ClosedBy { get; set; }

        public LoanAccount? LoanAccount { get; set; }
        public string? RazorpayOrderId { get; set; }
        public string? RazorpayPaymentId { get; set; }


    }
}
