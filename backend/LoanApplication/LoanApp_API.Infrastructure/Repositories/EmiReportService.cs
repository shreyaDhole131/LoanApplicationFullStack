using LoanApp_API.Application.DTO.ReportsDTOs;
using LoanApp_API.Application.Interfaces.Reports;
using LoanApp_API.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace LoanApp_API.Infrastructure.Repositories
{
    public class EmiReportService : IEmiReport
    {
        AppDbContext db;
        public EmiReportService(AppDbContext db)
        {
            this.db = db;
        }

        public async Task<List<EmiReportDTO>> fetchEmiReport(DateOnly? FromDate, DateOnly? ToDate)
        {
            var query = db.EmiSchedules.Where(e => e.PaymentStatus != "Cancelled");

            if (FromDate.HasValue)
            {
                var from = FromDate.Value.ToDateTime(TimeOnly.MinValue);
                query = query.Where(e => e.DueDate >= from);
            }

            if (ToDate.HasValue)
            {
                var to = ToDate.Value.ToDateTime(TimeOnly.MinValue).AddDays(-1);
                query = query.Where(e => e.DueDate < to);
            }

            var rows = await query.Select(e => new {e.Emi, e.PaymentStatus}).ToListAsync();

            var billedRows = rows;
            var collectedRows = rows.Where(r => r.PaymentStatus == "Paid").ToList();
            var pendingRows = rows.Where(r => r.PaymentStatus == "Pending").ToList();

            decimal billedAmount = billedRows.Sum(r => r.Emi);
            decimal collectedAmount = collectedRows.Sum(r => r.Emi);
            decimal pendingAmount = pendingRows.Sum(r => r.Emi);


            var result = new List<EmiReportDTO>
            {
                new EmiReportDTO
                {
                    Particulars = "Emi Billed",
                    Count = rows.Count,
                    Amount = billedAmount,
                    Percent = 100
                },
                new EmiReportDTO
                {
                    Particulars = "Emi Collected",
                    Count = collectedRows.Count,
                    Amount = collectedAmount,
                    Percent = billedAmount == 0 ? 0 : Math.Round((decimal) collectedAmount / billedAmount)
                },
                     new EmiReportDTO
                {
                    Particulars = "Emi Pending",
                    Count = pendingRows.Count,
                    Amount = pendingAmount,
                    Percent = 100
                }

            };

            return result;
        }
    }
}
