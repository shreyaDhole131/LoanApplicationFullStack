using LoanApp_API.Application.DTO;
using LoanApp_API.Application.Interfaces.Reports;
using LoanApp_API.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace LoanApp_API.Infrastructure.Repositories
{
    public class TatServiceReport : ITatReport
    {
        AppDbContext db;
        public TatServiceReport(AppDbContext db)
        {
            this.db = db;
        }

        public async Task<List<TatReportDTO>> fetchTatReport(DateOnly? fromDate, DateOnly? toDate)
        {
            var today = DateTime.UtcNow.Date;

            DateTime? from = fromDate?.ToDateTime(TimeOnly.MinValue);
            DateTime? to = toDate?.ToDateTime(TimeOnly.MinValue);

            var pendingQuery = db.LoanDeals.Where(d => d.CurrentStatus == "Pending");

            if (from.HasValue) pendingQuery = pendingQuery.Where(d => d.AppliedDate >= from.Value);

            if (to.HasValue) pendingQuery = pendingQuery.Where(d => d.AppliedDate < to.Value);

            var pendingDates = await pendingQuery.Select(d => d.AppliedDate).ToListAsync();

            var reviewDays = pendingDates.Select(d => (today - d.Date).TotalDays).Where(d => d >= 0).ToList();

            var disbQuery = db.LoanDeals.Where(d => d.disbursement != null);

            if (from.HasValue) disbQuery = disbQuery.Where(d => d.AppliedDate >= from.Value);
            if (to.HasValue) disbQuery = disbQuery.Where(d => d.AppliedDate < to.Value);

            var disbRows = await disbQuery.Select(d => new
            {
                d.AppliedDate,
                DisbursedDate = d.disbursement!.DisbursementDate
            }).ToListAsync();

            var disbDays = disbRows.Select(r => (r.DisbursedDate.Date - r.AppliedDate.Date).TotalDays)
                .Where(d => d >= 0).ToList();

            var result = new List<TatReportDTO>();

            result.Add(new TatReportDTO
            {
                Stage = "Application to Review",
                AvgDays = reviewDays.Count == 0 ? 0 : Math.Round((decimal)reviewDays.Average(), 1),
                MinDays = reviewDays.Count == 0 ? 0 : (int)reviewDays.Min(),
                MaxDays = reviewDays.Count == 0 ? 0 : (int)reviewDays.Max()
            });

            result.Add(new TatReportDTO
            {
                Stage = "Application to Disbursemrnt",
                AvgDays = disbDays.Count == 0 ? 0 : Math.Round((decimal)disbDays.Average(), 1),
                MinDays = disbDays.Count == 0 ? 0 : (int)disbDays.Min(),
                MaxDays = disbDays.Count == 0 ? 0 : (int)disbDays.Max()
            });

            var allDays = reviewDays.Concat(disbDays).ToList();

            result.Add(new TatReportDTO
            {
                Stage = "Total TAT",
                AvgDays = allDays.Count == 0 ? 0 : Math.Round((decimal)allDays.Average(), 1),
                MinDays = allDays.Count == 0 ? 0 : (int)allDays.Min(),
                MaxDays = allDays.Count == 0 ? 0 : (int)allDays.Max()
            });

            return result;

        }
    }
}
