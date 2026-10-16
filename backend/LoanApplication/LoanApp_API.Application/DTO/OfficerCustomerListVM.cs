namespace LoanApp_API.Application.DTO
{
    public class OfficerCustomerListVM
    {
        public int CustomerId { get; set; }

        public string Name { get; set; } = string.Empty;

        public string KycStatus { get; set; } = string.Empty;

        public int CibilScore { get; set; }

        public decimal LoanAmount { get; set; }

        public string LoanStatus { get; set; } = string.Empty;

        public DateTime? Date { get; set; }
    }
}