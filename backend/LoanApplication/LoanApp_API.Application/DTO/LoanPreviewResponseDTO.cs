using System;
using System.Collections.Generic;
using System.Text;

namespace LoanApp_API.Application.DTO
{
    public class LoanPreviewResponseDTO
    {
        public decimal InterestRate { get; set; }
        public decimal EmiAmount { get; set; }
    }
}
