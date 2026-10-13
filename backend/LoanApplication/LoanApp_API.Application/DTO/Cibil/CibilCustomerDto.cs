using System;
using System.Collections.Generic;
using System.Text;

namespace LoanApp_API.Application.DTO.Cibil
{
    public class CibilCustomerDto
    {
        public int CustomerId { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string MonthlyIncome { get; set; } = string.Empty;
    }

}
