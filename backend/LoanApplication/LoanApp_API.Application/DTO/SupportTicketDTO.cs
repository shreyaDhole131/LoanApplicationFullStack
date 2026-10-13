using System;
using System.Collections.Generic;
using System.Text;

namespace LoanApp_API.Application.DTO
{
    public class SupportTicketDTO
    {
        public int CustomerId { get; set; }
        public int LoanAccountId { get; set; }
        public string Subject { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string? AttachmentPath { get; set; }
    }
}
