
using LoanApp_API.Application.DTO.Eligibility;
using LoanApp_API.Application.Interfaces;
using LoanApp_API.Domain.Entities;
using LoanApp_API.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LoanApp_API.Infrastructure.Repositories
{
    public class EligibilityService : IEligibilityService
    {
        private readonly AppDbContext context;

        // Temporary assumption. Confirm the required FOIR later.
        private const decimal FoirLimit = 0.50m;
        private const decimal AnnualInterestRate = 11.5m;

        private static readonly int[] Tenures = { 36, 60, 72, 120 };

        public EligibilityService(AppDbContext context)
        {
            this.context = context;
        }

        public async Task<List<EligibilityCustomerDto>>
            GetManualReviewCustomersAsync()
        {
            var reports = await context.CibilReports
                .Include(r => r.Customer)
                .Where(r => r.CibilScore >= 650 &&
                            r.CibilScore <= 799)
                .ToListAsync();

            // If multiple reports exist, use only the latest
            // report for each customer.
            var latestReports = reports
                .Where(r => r.Customer != null)
                .GroupBy(r => r.CustomerId)
                .Select(g => g.OrderByDescending(r => r.CheckDate)
                              .ThenByDescending(r => r.CibilReportId)
                              .First());

            return latestReports.Select(r =>
                new EligibilityCustomerDto
                {
                    CustomerId = r.Customer!.CustomerId,
                    CustomerName = r.Customer.FirstName + " " +
                                   r.Customer.LastName,
                    Email = r.Customer.Email,
                    MonthlyIncome = decimal.TryParse(
                        r.Customer.MonthlyIncome, out decimal income)
                            ? income : 0m,
                    CibilScore = r.CibilScore,
                    ApplicationStatus = "Manual Review"
                }).ToList();
        }

        public async Task<EligibilityEvaluationDto?>
            EvaluateAndSaveAsync(int customerId)
        {
            var customer = await context.Customers
                .FirstOrDefaultAsync(c => c.CustomerId == customerId);

            if (customer == null)
                return null;

            var report = await context.CibilReports
                .Where(r => r.CustomerId == customerId)
                .OrderByDescending(r => r.CheckDate)
                .ThenByDescending(r => r.CibilReportId)
                .FirstOrDefaultAsync();

            if (report == null)
                return null;

            bool validIncome = decimal.TryParse(
                customer.MonthlyIncome, out decimal income)
                && income > 0;

            string status;
            string? rejectionReason = null;

            if (report.CibilScore >= 800)
            {
                status = "Auto Approved";
            }
            else if (report.CibilScore >= 650)
            {
                status = "Manual Review";
            }
            else
            {
                status = "Auto Rejected";
                rejectionReason = "CIBIL score is below 650.";
            }

            var activeLoans = await context.LoanAccounts
                .Where(l => l.CustomerId == customerId &&
                            l.LoanStatus == "Active")
                .ToListAsync();

            decimal existingEmi = activeLoans.Sum(l => l.EmiAmount);

            decimal maxAllowableEmi =
                validIncome ? income * FoirLimit : 0m;

            decimal availableEmi = Math.Max(
                0m, maxAllowableEmi - existingEmi);

            // Reuse the existing eligibility record.
            var saved = await context.EligibilityResults
                .FirstOrDefaultAsync(e => e.CustomerId == customerId);

            if (saved == null)
            {
                saved = new EligibilityResult
                {
                    CustomerId = customerId
                };

                context.EligibilityResults.Add(saved);
            }

            saved.CibilScore = report.CibilScore;
            saved.IsEligible = status;
            saved.RejectionReason = rejectionReason;
            saved.LoanAmount = 0m;

            await context.SaveChangesAsync();

            // Remove previous tenure calculations before recalculating.
            var oldTenures = await context.EligibilityTenureResults
                .Where(t => t.EligibilityId == saved.EligibilityId)
                .ToListAsync();

            context.EligibilityTenureResults.RemoveRange(oldTenures);

            var tenureDtos = new List<TenureEligibilityDto>();

            // Calculate tenure options only for Manual Review customers.
            if (status == "Manual Review" && validIncome)
            {
                decimal monthlyRate =
                    AnnualInterestRate / 100m / 12m;

                foreach (int months in Tenures)
                {
                    decimal amount = 0m;

                    if (availableEmi > 0)
                    {
                        double rate = (double)monthlyRate;

                        double principal =
                            (double)availableEmi *
                            (1 - Math.Pow(1 + rate, -months)) / rate;

                        amount = Math.Round((decimal)principal, 2);
                    }

                    var tenure = new EligibilityTenureResult
                    {
                        EligibilityId = saved.EligibilityId,
                        InterestRate = AnnualInterestRate,
                        TenureMonths = months,
                        ExistingMonthlyEmi = existingEmi,
                        MaxAllowableEmi = maxAllowableEmi,
                        AvailableEmi = availableEmi,
                        EligibleLoanAmount = amount,
                        CalculatedAt = DateTime.UtcNow
                    };

                    context.EligibilityTenureResults.Add(tenure);

                    tenureDtos.Add(new TenureEligibilityDto
                    {
                        TenureMonths = months,
                        InterestRate = AnnualInterestRate,
                        ExistingMonthlyEmi = existingEmi,
                        MaxAllowableEmi = maxAllowableEmi,
                        AvailableEmi = availableEmi,
                        EligibleLoanAmount = amount,
                        CalculatedAt = tenure.CalculatedAt
                    });
                }

                // Store the largest tenure amount in the existing
                // LoanAmount property.
                saved.LoanAmount = tenureDtos.Max(
                    t => t.EligibleLoanAmount);
            }

            await context.SaveChangesAsync();

            var storedTenures = await context.EligibilityTenureResults
                .Where(t => t.EligibilityId == saved.EligibilityId)
                .OrderBy(t => t.TenureMonths)
                .ToListAsync();

            tenureDtos = storedTenures.Select(t =>
                new TenureEligibilityDto
                {
                    EligibilityTenureResultId =
                        t.EligibilityTenureResultId,
                    TenureMonths = t.TenureMonths,
                    InterestRate = t.InterestRate,
                    ExistingMonthlyEmi = t.ExistingMonthlyEmi,
                    MaxAllowableEmi = t.MaxAllowableEmi,
                    AvailableEmi = t.AvailableEmi,
                    EligibleLoanAmount = t.EligibleLoanAmount,
                    CalculatedAt = t.CalculatedAt
                }).ToList();

            return new EligibilityEvaluationDto
            {
                EligibilityId = saved.EligibilityId,
                CustomerId = customerId,
                CibilScore = report.CibilScore,
                ApplicationStatus = status,
                LoanAmount = saved.LoanAmount,
                RejectionReason = saved.RejectionReason,
                MonthlyIncome = validIncome ? income : 0m,
                ExistingMonthlyEmi = existingEmi,
                MaxAllowableEmi = maxAllowableEmi,
                AvailableEmi = availableEmi,
                InterestRate = AnnualInterestRate,
                TenureOptions = tenureDtos
            };
        }

        public async Task<EligibilityEvaluationDto?>
            GetSavedResultAsync(int customerId)
        {
            var saved = await context.EligibilityResults
                .FirstOrDefaultAsync(e => e.CustomerId == customerId);

            if (saved == null)
                return null;

            var customer = await context.Customers
                .FirstOrDefaultAsync(c => c.CustomerId == customerId);

            var tenures = await context.EligibilityTenureResults
                .Where(t => t.EligibilityId == saved.EligibilityId)
                .OrderBy(t => t.TenureMonths)
                .ToListAsync();

            decimal.TryParse(customer?.MonthlyIncome, out decimal income);
            var first = tenures.FirstOrDefault();

            return new EligibilityEvaluationDto
            {
                EligibilityId = saved.EligibilityId,
                CustomerId = saved.CustomerId,
                CibilScore = saved.CibilScore,
                ApplicationStatus = saved.IsEligible ?? "Pending",
                LoanAmount = saved.LoanAmount,
                RejectionReason = saved.RejectionReason,
                MonthlyIncome = income,
                ExistingMonthlyEmi = first?.ExistingMonthlyEmi ?? 0m,
                MaxAllowableEmi = first?.MaxAllowableEmi ?? 0m,
                AvailableEmi = first?.AvailableEmi ?? 0m,
                InterestRate = first?.InterestRate ?? AnnualInterestRate,
                TenureOptions = tenures.Select(t =>
                    new TenureEligibilityDto
                    {
                        EligibilityTenureResultId =
                            t.EligibilityTenureResultId,
                        TenureMonths = t.TenureMonths,
                        InterestRate = t.InterestRate,
                        ExistingMonthlyEmi = t.ExistingMonthlyEmi,
                        MaxAllowableEmi = t.MaxAllowableEmi,
                        AvailableEmi = t.AvailableEmi,
                        EligibleLoanAmount = t.EligibleLoanAmount,
                        CalculatedAt = t.CalculatedAt
                    }).ToList()
            };
        }
    }
}
