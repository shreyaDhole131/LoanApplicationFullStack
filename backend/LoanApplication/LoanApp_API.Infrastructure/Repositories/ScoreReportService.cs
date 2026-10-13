using LoanApp_API.Application.DTO.ReportsDTOs;
using LoanApp_API.Application.Interfaces.Reports;
using LoanApp_API.Domain.Entities;
using LoanApp_API.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace LoanApp_API.Infrastructure.Repositories
{
    public class ScoreReportService :IScoreReport
    {
        AppDbContext db;
        public ScoreReportService(AppDbContext db)
        {
            this.db = db;
        }

        public async Task<List<ScoreCardDTO>> fetchScoreCardReport(DateOnly? fromDate, DateOnly? toDate)
        {

            var loanquery = db.LoanDeals.AsQueryable();

            if (fromDate.HasValue)
            {
                var from = fromDate.Value.ToDateTime(TimeOnly.MinValue);
                loanquery = loanquery.Where(ld => ld.AppliedDate >= from);
            }

            if (toDate.HasValue)
            {
                var to = toDate.Value.ToDateTime(TimeOnly.MinValue).AddDays(1);
                loanquery = loanquery.Where(ld => ld.AppliedDate < to);
            }

            var loanRows = await loanquery.Select(ld => new { ld.CustomerId, ld.CurrentStatus }).ToListAsync();

            var customerIds = loanRows.Select(l => l.CustomerId).Distinct().ToList();


            var scoreRows = await db.ScoreCards.Where(s => customerIds.Contains(s.CustomerId))
                .Select(s => new { s.CustomerId, s.RiskCategory }).ToListAsync();

            var approvedSet = loanRows.Where(l => l.CurrentStatus == "Approved").Select(l => l.CustomerId).ToHashSet();
            var rejectedSet = loanRows.Where(l => l.CurrentStatus == "Rejected").Select(l => l.CustomerId).ToHashSet();
            var pendingSet = loanRows.Where(l => l.CurrentStatus == "Pending").Select(l => l.CustomerId).ToHashSet();

            var categoryOrder = new[]
            {
                "Excellent",
                "Very Good",
                "Good",
                "Average",
                "Risky",
                "Reject"
            };

            var result = new List<ScoreCardDTO>();

            foreach (var category in categoryOrder)
            {
                var customersInCategroy = scoreRows.Where(s => s.RiskCategory == category).Select(s => s.CustomerId).Distinct().ToList();

                int applicants = customersInCategroy.Count();
                int approved = customersInCategroy.Count(id => approvedSet.Contains(id));
                int rejected = customersInCategroy.Count(id => rejectedSet.Contains(id));
                int pending = customersInCategroy.Count(id => pendingSet.Contains(id));

                decimal percent = applicants == 0 ? 0 : Math.Round((decimal)approved / applicants * 100, 2);
                result.Add(new ScoreCardDTO
                {
                    ScoreCard = category,
                    Applicants = applicants,
                    Approved = approved,
                    Rejected = rejected,
                    Pending = pending,
                    Approval = percent
                });


            }

            int totalApplicants = result.Sum(r => r.Applicants);
            int totalApproved = result.Sum(r => r.Approved);
            int totalRejected = result.Sum(r => r.Rejected);
            int totalPending = result.Sum(r => r.Pending);

            decimal totalPercent = totalApplicants == 0 ? 0 : Math.Round((decimal)totalApproved / totalApplicants * 100);

            result.Add(new ScoreCardDTO
            {
                ScoreCard = "Total",
                Applicants = totalApplicants,
                Approved = totalApproved,
                Rejected = totalRejected,
                Pending = totalPending,
                Approval = totalPercent
            });

            return result;

        }
    }
}
