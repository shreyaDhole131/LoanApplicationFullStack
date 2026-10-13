using LoanApp_API.Application.DTO.ReportsDTOs;
using LoanApp_API.Application.Interfaces.Reports;
using LoanApp_API.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace LoanApp_API.Infrastructure.Repositories
{
    public class CibiReportService : ICibilReport
    {
        AppDbContext db;

        public CibiReportService(AppDbContext db)
        {
            this.db = db;
        }

        public async Task<List<CibiReportDTO>> fetchCibilReport(DateOnly? fromDate, DateOnly? toDate)
        {
            var cibilQuery = db.CibilReports.AsQueryable();

            if (fromDate.HasValue)
            {
                var from = fromDate.Value.ToDateTime(TimeOnly.MinValue);
                cibilQuery = cibilQuery.Where(c => c.CheckDate >= from);
            }

            if (toDate.HasValue)
            { 
                var to = toDate.Value.ToDateTime(TimeOnly.MinValue).AddDays(1);
                cibilQuery = cibilQuery.Where(c => c.CheckDate < to);
            }

            var cibilRows = await cibilQuery.Select(c => new { c.CustomerId, c.CibilScore }).ToListAsync();

            var customerIds = cibilRows.Select(c => c.CustomerId).ToList();

            var approvedIds = await db.LoanDeals.Where(ld => customerIds.Contains(ld.CustomerId) && ld.CurrentStatus == "Approved")
                .Select(ld => ld.CustomerId).Distinct().ToListAsync();

            var approvedSet = approvedIds.ToHashSet();

            var ranges = new (string label, int Min, int Max)[]
            {
                ("800-899",800,899),
                ("750-799",750,799),
                ("700-749",700,749),
                ("650-699",650,699)
            };

            var result = new List<CibiReportDTO>();

            foreach (var range in ranges) 
            {
                var customersInRange = cibilRows.Where(c => c.CibilScore >= range.Min && c.CibilScore <= range.Max)
                    .Select(c => c.CustomerId)
                    .Distinct()
                    .ToList();

                int applicants = customersInRange.Count;
                int approved = customersInRange.Count(id => approvedSet.Contains(id));

                decimal percent = applicants == 0 ? 0 : Math.Round((decimal)approved / applicants);

                result.Add(new CibiReportDTO
                {
                    CibilScoreRange = range.label,
                    Applicants = applicants,
                    Approved = approved,
                    Approval = percent
                });
            }

            int totalApplicants = result.Sum(r => r.Applicants);
            int totalApproved = result.Sum(r => r.Approved);
            decimal totalApproval = totalApplicants == 0 ? 0 : Math.Round((decimal)totalApproved / totalApplicants,2);


            result.Add(new CibiReportDTO
            {
                CibilScoreRange = "Total",
                Applicants = totalApplicants,
                Approved = totalApproved,
                Approval = totalApproval
            });


            return result;




        }
    }
}
