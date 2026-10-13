using System;
using System.Collections.Generic;

namespace LoanApp_API.Application.DTO
{
    public class OfficerDashboardVM
    {
        public OfficerApplicationSummaryVM ApplicationSummary { get; set; } = new();

        public OfficerLoanSummaryVM LoanSummary { get; set; } = new();

        public List<OfficerApplicationFunnelVM> ApplicationFunnel { get; set; } = new();

        public List<OfficerLoanPortfolioVM> LoanPortfolio { get; set; } = new();

        public OfficerCollectionEfficiencyVM CollectionEfficiency { get; set; } = new();
    }

    public class OfficerApplicationSummaryVM
    {
        public int TotalApplications { get; set; }

        public int ApprovedApplications { get; set; }

        public int RejectedApplications { get; set; }

        public int PendingApplications { get; set; }
    }

    public class OfficerLoanSummaryVM
    {
        public int TotalLoans { get; set; }

        public int ActiveLoans { get; set; }

        public int ClosedLoans { get; set; }
    }

    public class OfficerApplicationFunnelVM
    {
        public string Stage { get; set; } = string.Empty;

        public int Count { get; set; }
    }

    public class OfficerLoanPortfolioVM
    {
        public string LoanType { get; set; } = string.Empty;

        public decimal TotalLoanAmount { get; set; }

        public int LoanCount { get; set; }
    }

    public class OfficerCollectionEfficiencyVM
    {
        public decimal ExpectedAmount { get; set; }

        public decimal CollectedAmount { get; set; }

        public decimal CollectionEfficiencyPercentage { get; set; }
    }
}