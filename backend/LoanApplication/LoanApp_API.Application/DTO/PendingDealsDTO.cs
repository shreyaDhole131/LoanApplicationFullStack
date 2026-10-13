using System;
using System.Collections.Generic;
using System.Text;

namespace LoanApp_API.Application.DTO
{
    public class PendingDealsDTO
    {
        public int DealId { get; set; }
        public string CustomerName { get; set; }

        public string LoanType { get; set; }

        public decimal LoanAmount { get; set; }

        public string Status { get; set; }

        public DateTime AppliedDate { get; set; }
    }
}
