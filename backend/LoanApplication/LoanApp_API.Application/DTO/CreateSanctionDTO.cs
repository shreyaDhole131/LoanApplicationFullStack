using System;
using System.Collections.Generic;
using System.Text;

namespace LoanApp_API.Application.DTO
{
    public class CreateSanctionDTO
    {
        public int DealId { get; set; }
        public decimal LoanAmount { get; set; }
        public int InterestRate { get; set; }
        public int TenureMonths { get; set; }
        public decimal EmiAmount { get; set; }
    }
}
