using AutoMapper;
using LoanApp_API.Application.DTO;
using LoanApp_API.Application.Interfaces;
using LoanApp_API.Application.Mapper;
using LoanApp_API.Domain.Entities;
using LoanApp_API.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace LoanApp_API.Infrastructure.Repositories
{
    public class LoanDealsService : ILoanDeals
    {
        AppDbContext db;
        IMapper mapper;

        public LoanDealsService(AppDbContext db, IMapper mapper)
        {
            this.db = db;
            this.mapper = mapper;

        }

        public async Task<LoanDeals> applyLoan(LoanDealDTO dto, int userId)
        {
            var customerId = await db.Users.Where(u => u.UserId == userId)
                .Select(u => u.CustomerId).FirstOrDefaultAsync();

            if (customerId == null)
                throw new UnauthorizedAccessException("User is not a customer");

            decimal rate = dto.TenureMonths switch
            {
                36 => 11.5m,
                60 => 11m,
                72 => 11m,
                120 => 11m,
                _ => throw new InvalidOperationException($"Tenure {dto.TenureMonths} not supported")
            };

            decimal newEmi = CalculateEmi(dto.LoanAmount, rate, dto.TenureMonths);

            var monthlyIncome = await db.Customers
                    .Where(c => c.CustomerId == customerId.Value)
                    .Select(c => c.MonthlyIncome)
                    .FirstOrDefaultAsync();

            decimal income = decimal.TryParse(monthlyIncome, out var inc) ? inc : 0m;

            decimal maxAllowableEmi = income * 0.40m;

            decimal existingEmi = await db.LoanDeals
               .Where(d => d.CustomerId == customerId.Value
                        && d.CurrentStatus == "Approved")
               .SumAsync(d => d.EmiAmount);

            decimal availableEmi = maxAllowableEmi - existingEmi;

            if (newEmi > availableEmi)
                throw new InvalidOperationException(
                    $"EMI {newEmi} exceeds available limit {availableEmi}");

            var deals = mapper.Map<LoanDeals>(dto);

            deals.CurrentStatus = "Pending";
            deals.EmiAmount = newEmi;
            deals.InterestRate = (int)rate;
            deals.CustomerId = customerId.Value;
            deals.AppliedDate = DateTime.UtcNow;
            deals.ApprovedAmount = 0;
            await db.LoanDeals.AddAsync(deals);
            await db.SaveChangesAsync();

            return deals;

        }


        private static decimal CalculateEmi(decimal principal, decimal annualRate, int months)
        {
            if (principal <= 0 || months <= 0) return 0m;

            double p = (double)principal;
            double r = (double)annualRate / 12.0 / 100.0;
            double n = months;

            double factor = Math.Pow(1 + r, n);
            double emi = p * r * factor / (factor - 1);

            return Math.Round((decimal)emi, 2);
        }

        public LoanPreviewResponseDTO CalculatePreview(decimal loanAmount, int tenureMonths)
        {
            decimal rate = tenureMonths switch
            {
                36 => 11.5m,
                60 => 11m,
                72 => 11m,
                120 => 11m,
                _ => throw new InvalidOperationException($"Tenure {tenureMonths} not supported")
            };

            decimal emi = CalculateEmi(loanAmount, rate, tenureMonths);

            return new LoanPreviewResponseDTO
            {
                InterestRate = rate,
                EmiAmount = emi
            };
        }
    }
}
