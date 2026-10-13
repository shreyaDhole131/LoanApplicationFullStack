using System;
using System.Collections.Generic;
using System.Text;

namespace LoanApp_API.Application.DTO.PaymentHistory
{
    public class PaymentHistoryDto
    {
        public int PaymentId { get; set; }
        public DateTime PaymentDate { get; set; }
        public string LoanAccountNo { get; set; } = "";
        public decimal PaidAmount { get; set; }
        public string PaymentMode { get; set; } = "";
        public string TransactionReference { get; set; } = "";
        public string PaymentStatus { get; set; } = "";
    }
}
