using LoanApp_API.Application.DTO.ReportsDTOs;
using LoanApp_API.Application.Interfaces.Reports;
using LoanApp_API.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace LoanApp_API.Infrastructure.Repositories
{
    public class ProductWiseReportService : IProductWiseReport
    {
        AppDbContext db;
        public ProductWiseReportService(AppDbContext db)
        {
            this.db = db;
        }

        public async Task<List<ProductReportDTO>> fetchProductReport(DateOnly? fromDate, DateOnly? toDate)
        {

            var query = db.LoanDeals.AsQueryable();

            if (fromDate.HasValue)
            {
                var from = fromDate.Value.ToDateTime(TimeOnly.MinValue);
                query = query.Where(ld => ld.AppliedDate >= from);
            }

            if (toDate.HasValue)
            {
                var to = toDate.Value.ToDateTime(TimeOnly.MinValue).AddDays(1);
                query = query.Where(ld => ld.AppliedDate < to);
            }

            var deals = await query.Select(ld => new
            {
                ld.DealId,
                ld.LoanType,
                ld.CurrentStatus,
                IsDisbursed = ld.disbursement != null
            }).ToListAsync();


            var productOrder = new[]
            {
                "Education Loan",
                "Home Loan",
                "Personal Loan"
            };


            var result = new List<ProductReportDTO>();

            foreach (var product in productOrder)
            {
                var dealsInProduct = deals.Where(d => d.LoanType == product).ToList();
                int applicants = dealsInProduct.Count();
                int approved = dealsInProduct.Count(d => d.CurrentStatus == "Approved");
                int disbursed = dealsInProduct.Count(d => d.IsDisbursed);

                decimal percent = applicants == 0 ? 0 : Math.Round((decimal)disbursed / applicants * 100, 2);

                result.Add(new ProductReportDTO
                {
                    Product = product,
                    Applicants = applicants,
                    Approved = approved,
                    Disbursed = disbursed,
                    DisbursedPercent = percent
                });
            }

            int totalApplicants = result.Sum(r => r.Applicants);
            int totalApproved = result.Sum(r => r.Approved);
            int totalDisbursed = result.Sum(r => r.Disbursed);

            decimal totalPrecent = totalApplicants == 0 ? 0 : Math.Round((decimal)totalDisbursed / totalApplicants * 100, 2);

            result.Add(new ProductReportDTO
            {
                Product = "Total",
                Applicants = totalApplicants,
                Approved = totalApproved,
                Disbursed = totalDisbursed,
                DisbursedPercent = totalPrecent
            });

            return result;
        }
    }
}
