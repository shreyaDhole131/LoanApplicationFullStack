using LoanApp_API.Application.DTO;
using LoanApp_API.Application.Interfaces;
using LoanApp_API.Infrastructure.Data;

namespace LoanApp_API.Infrastructure.Repositories
{
    public class OfficerDashboardRepository : IOfficerDashboardRepository
    {
        private readonly AppDbContext _db;

        public OfficerDashboardRepository(AppDbContext db)
        {
            _db = db;
        }

        public OfficerDashboardVM GetOfficerDashboard()
        {
            var deals = _db.LoanDeals.ToList();

            var loanAccounts = _db.LoanAccounts.ToList();

            var emiSchedules = _db.EmiSchedules.ToList();

            var dashboard = new OfficerDashboardVM();

            dashboard.ApplicationSummary = new OfficerApplicationSummaryVM
            {
                TotalApplications = deals.Count,

                ApprovedApplications = deals
                    .Count(x => x.CurrentStatus == "Approved"),

                RejectedApplications = deals
                    .Count(x => x.CurrentStatus == "Rejected"),

                PendingApplications = deals
                    .Count(x =>
                        x.CurrentStatus != "Approved" &&
                        x.CurrentStatus != "Rejected")
            };

            dashboard.LoanSummary = new OfficerLoanSummaryVM
            {
                TotalLoans = loanAccounts.Count,

                ActiveLoans = loanAccounts
                    .Count(x => x.LoanStatus == "Active"),

                ClosedLoans = loanAccounts
                    .Count(x => x.LoanStatus == "Closed")
            };

            dashboard.ApplicationFunnel = deals
                .GroupBy(x => x.CurrentStatus)
                .Select(x => new OfficerApplicationFunnelVM
                {
                    Stage = x.Key,
                    Count = x.Count()
                })
                .ToList();

            dashboard.LoanPortfolio = loanAccounts
                .Join(
                    deals,
                    account => account.DealId,
                    deal => deal.DealId,
                    (account, deal) => new
                    {
                        deal.LoanType,
                        account.LoanAmount
                    })
                .GroupBy(x => x.LoanType)
                .Select(x => new OfficerLoanPortfolioVM
                {
                    LoanType = x.Key,

                    TotalLoanAmount = x.Sum(y => y.LoanAmount),

                    LoanCount = x.Count()
                })
                .ToList();

            var expectedAmount = emiSchedules
                .Sum(x => x.Emi);

            var collectedAmount = emiSchedules
                .Where(x => x.PaymentStatus == "Paid")
                .Sum(x => x.Emi);

            decimal collectionEfficiency = 0;

            if (expectedAmount > 0)
            {
                collectionEfficiency =
                    (collectedAmount / expectedAmount) * 100;
            }

            dashboard.CollectionEfficiency =
                new OfficerCollectionEfficiencyVM
                {
                    ExpectedAmount = expectedAmount,

                    CollectedAmount = collectedAmount,

                    CollectionEfficiencyPercentage =
                        Math.Round(collectionEfficiency, 2)
                };

            return dashboard;
        }
    }
}