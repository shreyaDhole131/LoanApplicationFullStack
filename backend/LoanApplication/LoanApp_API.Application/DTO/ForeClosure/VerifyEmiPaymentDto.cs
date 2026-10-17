using System;
using System.Collections.Generic;
using System.Text;


namespace LoanApp_API.Application.DTO.EmiPayment
{
    public class VerifyEmiPaymentDto
    {
        public int EmiScheduleId { get; set; }
        public string RazorpayOrderId { get; set; } = string.Empty;
        public string RazorpayPaymentId { get; set; } = string.Empty;
        public string RazorpaySignature { get; set; } = string.Empty;
    }
}

