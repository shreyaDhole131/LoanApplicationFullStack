
using System.Collections.Generic;

namespace LoanApp_API.Application.DTO
{
    // Main Officer Dashboard View Model
    public class OfficerDashboardVM
    {
        public OfficerApplicationSummaryVM ApplicationSummary { get; set; } = new();

        public OfficerLoanSummaryVM LoanSummary { get; set; } = new();

        public List<OfficerApplicationFunnelVM> ApplicationFunnel { get; set; } = new();

        public List<OfficerLoanPortfolioVM> LoanPortfolio { get; set; } = new();

        public OfficerCollectionEfficiencyVM CollectionEfficiency { get; set; } = new();
    }

    // 1. Application Summary - 4 Cards
    public class OfficerApplicationSummaryVM
    {
        public int TotalApplications { get; set; }

        public int ApprovedApplications { get; set; }

        public int RejectedApplications { get; set; }

        public int PendingApplications { get; set; }
    }

    // 2. Loan Summary - 3 Cards
    public class OfficerLoanSummaryVM
    {
        public int TotalLoans { get; set; }

        public int ActiveLoans { get; set; }

        public int ClosedLoans { get; set; }
    }

    // 3. Application Funnel Chart
    public class OfficerApplicationFunnelVM
    {
        public string Stage { get; set; } = string.Empty;

        public int Count { get; set; }
    }

    // 4. Loan Portfolio - Pie Chart Response
    public class OfficerLoanPortfolioVM
    {
        public string LoanType { get; set; } = string.Empty;

        public decimal TotalLoanAmount { get; set; }

        public int LoanCount { get; set; }
    }

    // 5. Collection Efficiency - Bar Chart
    public class OfficerCollectionEfficiencyVM
    {
        public decimal ExpectedAmount { get; set; }

        public decimal CollectedAmount { get; set; }

        public decimal CollectionEfficiencyPercentage { get; set; }
    }

    // 6. Loan Portfolio - Internal Grouped Data for AutoMapper
    public class OfficerLoanPortfolioData
    {
        public string LoanType { get; set; } = string.Empty;

        public decimal TotalLoanAmount { get; set; }

        public int LoanCount { get; set; }
    }
}
