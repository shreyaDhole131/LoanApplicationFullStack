using System;
using System.Collections.Generic;
using System.Text;

namespace LoanApp_API.Application.DTO
{
   public class EmiScheduleDto
    {
        public int InstallmentNo { get; set; }
        public DateTime DueDate { get; set; }
        public decimal Emi { get; set; }
        public decimal PrincipalAmount { get; set; }
        public decimal InterestAmount { get; set; }
        public decimal OpeningBalance { get; set; }
        public decimal ClosingBalance { get; set; }
        public string PaymentStatus { get; set; }
    }
}
