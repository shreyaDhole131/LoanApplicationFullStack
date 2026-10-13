using LoanApp_API.Application.DTO.ReportsDTOs;
using LoanApp_API.Application.Interfaces.Reports;
using LoanApp_API.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace LoanApp_API.Infrastructure.Repositories
{
    public class DisburseReportService : IDisburseReport
    {
        AppDbContext db;
        public DisburseReportService(AppDbContext db)
        {
            this.db = db;
        }

        public async Task<List<DisbursementReportDTO>> fetchDisbursmentReport(DateOnly? fromDate, DateOnly? toDate)
        {
            var query = db.Disbursements.AsQueryable();

            if (fromDate.HasValue)
            {
                var from = fromDate.Value.ToDateTime(TimeOnly.MinValue);
                query = query.Where(d => d.DisbursementDate >= from);
            }

            if (toDate.HasValue)
            {
                var to = toDate.Value.ToDateTime(TimeOnly.MinValue).AddDays(1);
                query = query.Where(d => d.DisbursementDate < to);
            }

            var rows = await query.Select(d => new { d.DisbursementDate, d.DisburseAmount }).ToListAsync();

            var monthly = rows.GroupBy(r => new { r.DisbursementDate.Year, r.DisbursementDate.Month })
                .Select(g => new DisbursementReportDTO
                {
                    Month = new DateTime(g.Key.Year, g.Key.Month, 1).ToString("MMM"),
                    LoanDisbursed = g.Count(),
                    DecimalAmount = g.Sum(x => x.DisburseAmount)
                })
                .OrderBy(r => r.Month)
                .ToList();

            var total = new DisbursementReportDTO
            {
                Month = "Total",
                LoanDisbursed = monthly.Sum(r => r.LoanDisbursed),
                DecimalAmount = monthly.Sum(r => r.DecimalAmount)
            };

            monthly.Add(total);

            return monthly;

        }
    }
}
