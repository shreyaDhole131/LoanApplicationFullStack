namespace LoanApp_API.Application.DTO.ScoreCard
{
    public class ScoreCardProfileDto
    {
        public int ScoreCardId { get; set; }
        public string RiskCategory { get; set; } = string.Empty;
        public decimal EligibleLoanAmount { get; set; }
    }
}