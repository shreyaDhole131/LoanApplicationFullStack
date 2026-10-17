using System;
using System.Collections.Generic;
using System.Text;


namespace LoanApp_API.Application.DTO.EmiPayment
{
    public class EmiPaymentDto
    {
        public int EmiScheduleId { get; set; }

        public int InstallmentNo { get; set; }

        public DateTime DueDate { get; set; }

        public decimal Emi { get; set; }

        public decimal Penalty { get; set; }

        public decimal BounceCharges { get; set; }

        public decimal GST { get; set; }

        public decimal TotalAmount { get; set; }

        public string PaymentStatus { get; set; } = string.Empty;
    }
}
