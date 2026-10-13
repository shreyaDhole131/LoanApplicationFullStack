using System;
using System.Collections.Generic;
using System.Text;

namespace LoanApp_API.Application.DTO
{
    public class DisbursePrefillDTO
    {
        public int DealId { get; set; }
        public decimal DisburseAmount { get; set; }
        public DateTime DisbursementDate { get; set; }
    }
}
