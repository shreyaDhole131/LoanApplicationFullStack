using System;
using System.Collections.Generic;
using System.Text;

namespace LoanApp_API.Application.DTO
{
    public class EmiScheduleSummaryDto
    {
        public int PaidEmiCount { get; set; }
        public decimal PaidEmiAmount { get; set; }

        public int PendingEmiCount { get; set; }
        public decimal PendingEmiAmount { get; set; }

        public int RemainingEmiCount { get; set; }
        public decimal RemainingBalance { get; set; }

        public List<EmiScheduleDto> Schedules { get; set; } = new();
    }
}
