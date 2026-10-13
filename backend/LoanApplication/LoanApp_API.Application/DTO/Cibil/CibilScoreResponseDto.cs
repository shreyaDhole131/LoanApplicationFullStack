using System;
using System.Collections.Generic;
using System.Text;

namespace LoanApp_API.Application.DTO.Cibil
{
    public class CibilScoreResponseDto
    {
        public int CustomerId { get; set; }

        public string CustomerName { get; set; } = string.Empty;

        public string PanNo { get; set; } = string.Empty;

        public int CibilScore { get; set; }

        public string ScoreStatus { get; set; } = string.Empty;

        public DateTime CheckDate { get; set; }
    }
}
