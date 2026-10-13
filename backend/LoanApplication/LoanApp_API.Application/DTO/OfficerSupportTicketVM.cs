using System;

namespace LoanApp_API.Application.DTO
{
    public class OfficerSupportTicketVM
    {
        public int TicketId { get; set; }

        public int CustomerId { get; set; }

        public string CustomerName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public int LoanAccountId { get; set; }

        public string LoanAccountNo { get; set; } = string.Empty;

        public string Subject { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public string? AttachmentPath { get; set; }

        public DateTime CreatedDate { get; set; }

        public string Status { get; set; } = string.Empty;

        public string? OfficerResponse { get; set; }
    }

    public class OfficerSupportTicketUpdateDTO
    {
        public string Status { get; set; } = string.Empty;

        public string? OfficerResponse { get; set; }
    }
}