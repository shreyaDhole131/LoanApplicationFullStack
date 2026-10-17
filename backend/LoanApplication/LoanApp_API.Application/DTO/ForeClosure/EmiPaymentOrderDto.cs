using System;
using System.Collections.Generic;
using System.Text;


namespace LoanApp_API.Application.DTO.EmiPayment
{
    public class EmiPaymentOrderDto
    {
        public int EmiScheduleId { get; set; }
        public string OrderId { get; set; } = string.Empty;
        public long AmountInPaise { get; set; }
        public string Currency { get; set; } = "INR";
        public string KeyId { get; set; } = string.Empty;
    }
}
