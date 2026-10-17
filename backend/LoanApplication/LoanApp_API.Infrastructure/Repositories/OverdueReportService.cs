using LoanApp_API.Application.DTO;
using LoanApp_API.Application.Interfaces.Reports;
using LoanApp_API.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;


namespace LoanApp_API.Infrastructure.Repositories
{
    public class OverdueReportService : IOverdueReport
    {
        AppDbContext db;
        public OverdueReportService(AppDbContext db)
        {
            this.db = db;
        }

        public async Task<List<OverdueReportDTO>> fetchOverdueReport(DateOnly? asOnDate)
        {
            var asOn = asOnDate.HasValue
            ? asOnDate.Value.ToDateTime(TimeOnly.MinValue)
            : DateTime.UtcNow.Date;


            var rows = await db.EmiSchedules
                      .Where(e => e.PaymentStatus == "Pending" && e.DueDate < asOn)
                      .Select(e => new
                      {
                          e.LoanAccountId,
                          e.DueDate,
                          Outstanding = e.LoanAccount!.OutstandingPrincipal
                      }).ToListAsync();

            var buckets = new (string label, int Min, int Max)[]
                {
                    ("1 - 30 Days",    1,   30),
                    ("31 - 60 Days",   31,  60),
                    ("61 - 90 Days",   61,  90),
                    ("Above 90 Days",  91,  int.MaxValue)
                };


            var result = new List<OverdueReportDTO>();

            foreach (var b in buckets)
            {
                var rowsInBucket = rows.Where(r =>
                {
                    var days = (asOn - r.DueDate).Days;
                    return days >= b.Min && days <= b.Max;
                }).ToList();

                var distinctAccounts = rowsInBucket.
               GroupBy(rows => rows.LoanAccountId)
               .Select(global => global.First().Outstanding)
               .ToList();

                result.Add(new OverdueReportDTO
                {
                    DaysOverdue = b.label,
                    Accounts = distinctAccounts.Count,
                    Outstanding = distinctAccounts.Sum()
                });



            }

            result.Add(new OverdueReportDTO
            {
                DaysOverdue = "Total",
                Accounts = result.Sum(r => r.Accounts),
                Outstanding = result.Sum(r => r.Outstanding)
            });

            return result;




        }
    }
}
