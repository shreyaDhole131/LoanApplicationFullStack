using System;
using System.Collections.Generic;
using System.Text;

namespace LoanApp_API.Application.DTO.User
{
    public class CustomerRegistrationDTO
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public int Age { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string MobileNo { get; set; } = string.Empty;
        public string Pan { get; set; } = string.Empty;
        public string AadhaarNo { get; set; } = string.Empty;
        public string EmploymentType { get; set; } = string.Empty;
        public string MonthlyIncome { get; set; } = string.Empty;
    }
}
