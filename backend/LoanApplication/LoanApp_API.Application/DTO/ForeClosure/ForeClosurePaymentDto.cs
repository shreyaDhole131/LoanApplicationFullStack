using System;
using System.Collections.Generic;
using System.Text;

namespace LoanApp_API.Application.DTO.ForeClosure
{
    // Sent to frontend after the Razorpay order is created
    public class ForeClosureOrderDto
    {
        public int RequestId { get; set; }
        public string OrderId { get; set; } = string.Empty;
        public long AmountInPaise { get; set; }
        public string Currency { get; set; } = "INR";
        public string KeyId { get; set; } = string.Empty;
    }

    // Sent by frontend after the Razorpay payment succeeds
    public class VerifyForeClosurePaymentDto
    {
        public int RequestId { get; set; }
        public string RazorpayOrderId { get; set; } = string.Empty;
        public string RazorpayPaymentId { get; set; } = string.Empty;
        public string RazorpaySignature { get; set; } = string.Empty;
    }
}