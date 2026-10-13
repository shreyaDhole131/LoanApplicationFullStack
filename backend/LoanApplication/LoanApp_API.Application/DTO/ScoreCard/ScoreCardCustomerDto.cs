
namespace LoanApp_API.Application.DTO.ScoreCard
{
    public class ScoreCardCustomerDto
    {
        public int CustomerId { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string MonthlyIncome { get; set; } = string.Empty;
    }
}
