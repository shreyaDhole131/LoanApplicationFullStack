using AutoMapper;
using LoanApp_API.Application.DTO;
using LoanApp_API.Application.Interfaces;
using LoanApp_API.Domain.Entities;
using LoanApp_API.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace LoanApp_API.Infrastructure.Repositories
{
   public class EmiScheduleService : IEmiScheduleService
    {
        AppDbContext db;
        IMapper mapper;
        public EmiScheduleService(AppDbContext db, IMapper mapper)
        {
            this.db = db;
            this.mapper = mapper;
        }

        public async Task GenerateEMISchedule(int loanAccountId)
        {
            
            var alreadyExists = await db.EmiSchedules
                .AnyAsync(x => x.LoanAccountId == loanAccountId);

            if (alreadyExists)
                return;

          
            var loan = await db.LoanAccounts
                .FirstOrDefaultAsync(x => x.LoanAccountId == loanAccountId);

            if (loan == null)
                return;

           
            decimal principal = loan.LoanAmount;
            decimal annualRate = loan.InterestRate;
            int tenure = loan.TenureMonths;

            if (tenure <= 0)
                return;

           
            decimal monthlyRate = annualRate / 12 / 100;

          
            decimal emi;

            if (monthlyRate == 0)
            {
                emi = principal / tenure;
            }
            else
            {
                double rate = (double)monthlyRate;

                emi =
                    principal *
                    monthlyRate *
                    (decimal)Math.Pow(1 + rate, tenure)
                    /
                    ((decimal)Math.Pow(1 + rate, tenure) - 1);
            }

            emi = Math.Round(emi, 2);

        
            decimal openingBalance = principal;

            for (int i = 1; i <= tenure; i++)
            {
                decimal interest =
                    Math.Round(openingBalance * monthlyRate, 2);

                decimal principalAmount =
                    Math.Round(emi - interest, 2);

                decimal closingBalance =
                    Math.Round(
                        openingBalance - principalAmount,
                        2);

            
                if (i == tenure)
                {
                    principalAmount = openingBalance;
                    interest = Math.Round(
                        emi - principalAmount, 2);

                    closingBalance = 0;
                }

                var schedule = new EmiSchedule
                {
                    LoanAccountId = loanAccountId,

                    InstallmentNo = i,

                    DueDate = loan.DisbursementDate
                        .AddMonths(i),

                    Emi = emi,

                    PrincipalAmount = principalAmount,

                    InterestAmount = interest,

                    OpeningBalance = openingBalance,

                    ClosingBalance = closingBalance,

                    PaymentStatus = "Pending",

                    PaidDate = null
                };

                db.EmiSchedules.Add(schedule);

                openingBalance = closingBalance;
            }

         
            await db.SaveChangesAsync();
        }

        public async Task<EmiScheduleSummaryDto> GetEMISchedule(int loanAccountId)
        {
            var schedules = await db.EmiSchedules
                .Where(x => x.LoanAccountId == loanAccountId)
                .OrderBy(x => x.InstallmentNo)
                .ToListAsync();

            var paidSchedules = schedules
                .Where(x => x.PaymentStatus == "Paid")
                .ToList();

            var pendingSchedules = schedules
                .Where(x => x.PaymentStatus == "Pending")
                .ToList();

            var firstPending = pendingSchedules
               .OrderBy(x => x.InstallmentNo)
               .FirstOrDefault();

            return new EmiScheduleSummaryDto
            {
                PaidEmiCount = paidSchedules.Count,

                PaidEmiAmount = paidSchedules
                    .Sum(x => x.Emi),

                PendingEmiCount = pendingSchedules.Count,

                PendingEmiAmount = pendingSchedules
                    .Sum(x => x.Emi),

                RemainingEmiCount = pendingSchedules.Count,

                RemainingBalance = firstPending?.OpeningBalance ?? 0,

                //Schedules = schedules.Select(x => new EmiScheduleDto
                //{
                //    InstallmentNo = x.InstallmentNo,
                //    DueDate = x.DueDate,
                //    Emi = x.Emi,
                //    PrincipalAmount = x.PrincipalAmount,
                //    InterestAmount = x.InterestAmount,
                //    OpeningBalance = x.OpeningBalance,
                //    ClosingBalance = x.ClosingBalance,
                //    PaymentStatus = x.PaymentStatus
                //}).ToList()

                Schedules = mapper.Map<List<EmiScheduleDto>>(schedules)
            };
        }

    }
}
