
using AutoMapper;
using LoanApp_API.Application.DTO;
using LoanApp_API.Application.Interfaces;
using LoanApp_API.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LoanApp_API.Infrastructure.Repositories
{
    public class OfficerDashboardRepository : IOfficerDashboardRepository
    {
        private readonly AppDbContext _db;
        private readonly IMapper _mapper;

        public OfficerDashboardRepository(
            AppDbContext db,
            IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        public OfficerDashboardVM GetOfficerDashboard()
        {
            var deals = _db.LoanDeals
                .AsNoTracking()
                .ToList();

            var loanAccounts = _db.LoanAccounts
                .AsNoTracking()
                .ToList();

            var emiSchedules = _db.EmiSchedules
                .AsNoTracking()
                .ToList();

            var dashboard = new OfficerDashboardVM();

            // Application summary: 4 cards
            dashboard.ApplicationSummary =
                new OfficerApplicationSummaryVM
                {
                    TotalApplications = deals.Count,

                    ApprovedApplications = deals.Count(
                        x => x.CurrentStatus == "Approved"),

                    RejectedApplications = deals.Count(
                        x => x.CurrentStatus == "Rejected"),

                    PendingApplications = deals.Count(
                        x => x.CurrentStatus != "Approved"
                          && x.CurrentStatus != "Rejected")
                };

            // Loan summary: 3 cards
            dashboard.LoanSummary = new OfficerLoanSummaryVM
            {
                TotalLoans = loanAccounts.Count,

                ActiveLoans = loanAccounts.Count(
                    x => x.LoanStatus == "Active"),

                ClosedLoans = loanAccounts.Count(
                    x => x.LoanStatus == "Closed")
            };

            // Application funnel
            var funnelItems =
                _mapper.Map<List<OfficerApplicationFunnelVM>>(deals);

            dashboard.ApplicationFunnel = funnelItems
                .GroupBy(x => x.Stage)
                .Select(g => new OfficerApplicationFunnelVM
                {
                    Stage = g.Key,
                    Count = g.Sum(x => x.Count)
                })
                .OrderByDescending(x => x.Count)
                .ToList();

            // Loan portfolio: pie chart
            var portfolioData = loanAccounts
                .Join(
                    deals,
                    account => account.DealId,
                    deal => deal.DealId,
                    (account, deal) => new
                    {
                        LoanType = deal.LoanType ?? "Other",
                        account.LoanAmount
                    })
                .GroupBy(x => x.LoanType)
                .Select(g => new OfficerLoanPortfolioData
                {
                    LoanType = g.Key,
                    TotalLoanAmount = g.Sum(x => x.LoanAmount),
                    LoanCount = g.Count()
                })
                .ToList();

            dashboard.LoanPortfolio =
                _mapper.Map<List<OfficerLoanPortfolioVM>>(
                    portfolioData);

            // Collection efficiency
            decimal expectedAmount = emiSchedules.Sum(x => x.Emi);

            decimal collectedAmount = emiSchedules
                .Where(x => x.PaymentStatus == "Paid")
                .Sum(x => x.Emi);

            decimal efficiency = expectedAmount > 0
                ? collectedAmount / expectedAmount * 100
                : 0;

            dashboard.CollectionEfficiency =
                new OfficerCollectionEfficiencyVM
                {
                    ExpectedAmount = expectedAmount,
                    CollectedAmount = collectedAmount,
                    CollectionEfficiencyPercentage =
                        Math.Round(efficiency, 2)
                };

            return dashboard;
        }
    }
}
