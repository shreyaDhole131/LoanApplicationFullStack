using System;
using System.Collections.Generic;
using System.Text;




namespace LoanApp_API.Application.DTO.Eligibility
{
    public class EligibilityCustomerDto
    {
        public int CustomerId { get; set; }

        public string CustomerName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public decimal MonthlyIncome { get; set; }

        public int CibilScore { get; set; }

        public string ApplicationStatus { get; set; } = string.Empty;
    }
}


