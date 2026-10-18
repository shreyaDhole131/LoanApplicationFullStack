using System;
using System.Collections.Generic;
using System.Text;

namespace LoanApp_API.Application.DTO.ScoreCard
{
    public class ScoreCardDto
    {
        public int ScoreCardId { get; set; }

        public int CustomerId { get; set; }

        public string RiskCategory { get; set; } = string.Empty;

        public decimal EligibleLoanAmount { get; set; }
    }
}