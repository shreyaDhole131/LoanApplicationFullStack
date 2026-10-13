using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace LoanApp_API.Domain.Entities
{
    public class SupportTicket
    {
        [Key]
        public int TicketId { get; set; }

        [ForeignKey("Customer")]
        public int CustomerId { get; set; }

        [ForeignKey("LoanAccount")]
        public int LoanAccountId { get; set; }

        public string Subject { get; set; }

        public string Description { get; set; }

        public string? AttachmentPath { get; set; }

        public DateTime CreatedDate { get; set; }

        public string Status { get; set; }

        public string? OfficerResponse { get; set; }

        public Customers? Customer { get; set; }

        public LoanAccount? LoanAccount { get; set; }
    }
}
