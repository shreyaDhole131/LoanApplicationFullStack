using LoanApp_API.Application.DTO.ReportsDTOs;
using LoanApp_API.Application.Interfaces.Reports;
using LoanApp_API.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace LoanApp_API.Infrastructure.Repositories
{
    public class PrepaymentReportService : IPrepaymentReport
    {
        AppDbContext db;
        public PrepaymentReportService(AppDbContext db)
        {
            this.db = db;
        }

        public async Task<List<PrepaymentReportDTO>> fetchPrepaymentReport(DateOnly? fromDate, DateOnly? toDate)
        {
            var query = db.ForeClosureRequests.Where(f => f.IsPaid == true);

            if (fromDate.HasValue)
            {
                var from = fromDate.Value.ToDateTime(TimeOnly.MinValue);
                query = query.Where(f => f.PaidDate >= from);
            }

            if (toDate.HasValue)
            {
                var to = toDate.Value.ToDateTime(TimeOnly.MinValue).AddDays(1);
                query = query.Where(f => f.PaidDate < to);
            }

            var rows = await query.Select(f => new {
                f.ForeClosureType,
                f.ForeClosureAmount,
                f.PartialAmount
            }).ToListAsync();

            var result = new List<PrepaymentReportDTO>();

            var partRows = rows.Where(r => r.ForeClosureType == "Partial").ToList();

            result.Add(new PrepaymentReportDTO
            {
                Type = "Part PrePayment",
                Accounts = partRows.Count,
                PrepaymentAmount = partRows.Sum(r => r.PartialAmount)

            });

            var fullRows = rows.Where(r => r.ForeClosureType == "Full").ToList();

            result.Add(new PrepaymentReportDTO
            {
                Type = "Full PrePayment",
                Accounts = fullRows.Count,
                PrepaymentAmount = fullRows.Sum(r => r.ForeClosureAmount)
            });

            result.Add(new PrepaymentReportDTO
            {
                Type = "Total",
                Accounts = result.Sum(r => r.Accounts),
                PrepaymentAmount = result.Sum(r => r.PrepaymentAmount)
            });

            return result;

        }
    }
}
