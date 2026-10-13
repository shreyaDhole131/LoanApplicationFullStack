using LoanApp_API.Application.DTO;
using LoanApp_API.Application.Interfaces;
using LoanApp_API.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace LoanApp_API.Infrastructure.Repositories
{
    public class PendingDealsService : IPendingDeals
    {
        AppDbContext db;

        public PendingDealsService(AppDbContext db)
        {
            this.db = db;
        }

        public async Task<CustomerShowProfileDTO?> fetchCustomerProfile(int dealId)
        {
            return await db.LoanDeals.Include(ld => ld.customers).Where(ld => ld.DealId == dealId).Select(ld => new CustomerShowProfileDTO
            {
                DealId = ld.DealId,
                CustomerName = ld.customers.FirstName + " " + ld.customers.LastName,
                MobileNo = ld.customers.MobileNo,
                Pan = ld.customers.Pan,
                AadhaarNo = ld.customers.AadhaarNo,
                MonthlyIncome = ld.customers.MonthlyIncome,
                LoanType = ld.LoanType,
                LoanAmount = ld.LoanAmount,
                InterestRate = ld.InterestRate,
                TenureMonths = ld.TenureMonths,
                EmiAmount = ld.EmiAmount
            }).FirstOrDefaultAsync();
        }

        public async Task<List<PendingDealsDTO>> fetchPendingDeals()
        {
           return await db.LoanDeals.Include(ld => ld.customers).Where(ld => ld.CurrentStatus == "Pending").Select(ld => new PendingDealsDTO
            {
                DealId = ld.DealId,
                CustomerName = ld.customers.FirstName + " " + ld.customers.LastName,
                LoanType = ld.LoanType,
                LoanAmount = ld.LoanAmount,
               AppliedDate= ld.AppliedDate,
                Status = "Pending"
            }).ToListAsync();
        }

        public async Task reviewDeal(int dealId, string status)
        {
            var deal = await db.LoanDeals.FirstOrDefaultAsync(d => d.DealId == dealId);
            if (deal == null) throw new InvalidOperationException($"Deal {dealId} not found");
            deal.CurrentStatus = status;
            await db.SaveChangesAsync();

        }

        public async Task<List<PendingDealsDTO>> sortPendingDeals(string? LoanType, decimal? MinAmount, decimal? MaxAmount, string? dateFilter, DateOnly? FromFilter, DateOnly? ToFilter)
        {
            var query = db.LoanDeals.Where(ld => ld.CurrentStatus == "Pending");

            if (!string.IsNullOrWhiteSpace(LoanType))
            {
                query = query.Where(ld => ld.LoanType  == LoanType);
            }

            if (MinAmount.HasValue)
            {
                query = query.Where(ld => ld.LoanAmount >= MinAmount.Value);
            }

            if (MaxAmount.HasValue)
            {
                query = query.Where(ld => ld.LoanAmount <= MaxAmount.Value);
            }

            var Today = DateTime.UtcNow.Date;

            if(dateFilter == "Last 7 Days")
            {
                query =query.Where(ld => ld.AppliedDate >= Today.AddDays(-7));
            }
            else if (dateFilter == "Last 30 Days")
            {
                query=query.Where(ld => ld.AppliedDate >= Today.AddDays(-30));
            }
           

            if (FromFilter.HasValue)
            { 
                var from = FromFilter.Value.ToDateTime(TimeOnly.MinValue);
                query = query.Where(ld => ld.AppliedDate >=  from);

            }

            if (ToFilter.HasValue)
            {
                var to = ToFilter.Value.ToDateTime(TimeOnly.MinValue).AddDays(1);
                query = query.Where(ld => ld.AppliedDate < to);
            }

            query = dateFilter == "Ascending"
                ? query.OrderBy(ld => ld.AppliedDate)
                : query.OrderByDescending(ld => ld.AppliedDate);


            return await query.Select(ld => new PendingDealsDTO
            {
                DealId = ld.DealId,
                CustomerName = ld.customers.FirstName + " " + ld.customers.LastName,
                LoanType = ld.LoanType,
                LoanAmount = ld.LoanAmount,
                AppliedDate= ld.AppliedDate,
                Status = "Pending"
            }).ToListAsync();


        }
    }
}
