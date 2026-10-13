namespace LoanApp_API.Application.DTO
{
    public class UpdateCustomerProfileDTO
    {
        public string FirstName { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;

        public string MobileNo { get; set; } = string.Empty;

        public string EmploymentType { get; set; } = string.Empty;

        public string MonthlyIncome { get; set; } = string.Empty;

        public int Age { get; set; }
    }
}