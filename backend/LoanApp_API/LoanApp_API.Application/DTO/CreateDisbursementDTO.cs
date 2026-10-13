using System;
using System.Collections.Generic;
using System.Text;

namespace LoanApp_API.Application.DTO
{
    public class CreateDisbursementDTO
    {
        public int DealId { get; set; }
        public decimal DisburseAmount { get; set; }
        public string BankPartner { get; set; }
        public DateTime DisbursementDate { get; set; }
        public string Status { get; set; }
    }
}
