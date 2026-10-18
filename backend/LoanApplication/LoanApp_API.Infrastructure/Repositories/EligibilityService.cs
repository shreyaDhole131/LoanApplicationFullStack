using LoanApp_API.Application.DTO.Eligibility;
using LoanApp_API.Application.Interfaces;
using LoanApp_API.Domain.Entities;
using LoanApp_API.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace LoanApp_API.Infrastructure.Repositories
{
    public class EligibilityService : IEligibilityService
    {
        private readonly AppDbContext context;

        private const decimal FoirLimit = 0.50m;
        private const decimal AnnualInterestRate = 11.5m;
        // Annual interest rates and tenure periods.
        private static readonly (int TenureMonths, decimal InterestRate)[] TenurePlans =
        {
            (36, 11.5m),
            (60, 11m),
            (72, 11m),
            (120, 11m)
        };

        private const decimal MaximumFoirPercent = 40m;
        private const int AutomaticApprovalMinimumScore = 800;
        private const int ManualReviewMinimumScore = 650;

        public EligibilityService(AppDbContext context)
        {
            this.context = context;
        }

        // Convert the customer's monthly income string to decimal.
        private static decimal ParseIncome(string? monthlyIncome)
        {
            if (decimal.TryParse(
                monthlyIncome,
                NumberStyles.Any,
                CultureInfo.InvariantCulture,
                out decimal income))
            {
                return income;
            }

            if (decimal.TryParse(
                monthlyIncome,
                NumberStyles.Any,
                CultureInfo.CurrentCulture,
                out income))
            {
                return income;
            }

            return 0m;
        }

        // Check whether a decision is already final.
        private static bool IsFinalDecision(string? status)
        {
            return string.Equals(
                       status, "Approved",
                       StringComparison.OrdinalIgnoreCase)
                || string.Equals(
                       status, "Rejected",
                       StringComparison.OrdinalIgnoreCase)
                || string.Equals(
                       status, "Auto Approved",
                       StringComparison.OrdinalIgnoreCase)
                || string.Equals(
                       status, "Auto Rejected",
                       StringComparison.OrdinalIgnoreCase);
        }

        private static bool IsReviewableStatus(string? status)
        {
            return string.IsNullOrWhiteSpace(status)
                || string.Equals(
                    status, "Pending",
                    StringComparison.OrdinalIgnoreCase)
                || string.Equals(
                    status, "Manual Review",
                    StringComparison.OrdinalIgnoreCase);
        }

        // Fetch the latest CIBIL report for each customer.
        private async Task<List<CibilReport>> GetLatestReportsAsync()
        {
            var reports = await context.CibilReports
                .AsNoTracking()
                .Include(r => r.Customer)
                .ToListAsync();

            return reports
                .Where(r => r.Customer != null)
                .GroupBy(r => r.CustomerId)
                .Select(g => g
                    .OrderByDescending(r => r.CheckDate)
                    .ThenByDescending(r => r.CibilReportId)
                    .First())
                .ToList();
        }

        // Sum monthly EMI amounts from loan accounts that are not closed.
        // Do not sum EmiSchedules because they contain multiple instalments
        // for the same loan.
        private async Task<decimal> GetExistingMonthlyEmiAsync(
            int customerId)
        {
            var accounts = await context.LoanAccounts
                .AsNoTracking()
                .Where(l => l.CustomerId == customerId)
                .Select(l => new
                {
                    l.LoanStatus,
                    l.EmiAmount
                })
                .ToListAsync();

            return accounts
                .Where(l => !string.Equals(
                    l.LoanStatus?.Trim(),
                    "Closed",
                    StringComparison.OrdinalIgnoreCase))
                .Where(l => l.EmiAmount > 0m)
                .Sum(l => l.EmiAmount);
        }

        // Calculate the loan principal supported by a monthly EMI.
        // InterestRate is annual, so convert it to a monthly rate.
        private static decimal CalculateEligiblePrincipal(
            decimal availableEmi,
            decimal annualInterestRate,
            int tenureMonths)
        {
            if (availableEmi <= 0m || tenureMonths <= 0)
            {
                return 0m;
            }

            decimal monthlyRate = annualInterestRate / 1200m;

            if (monthlyRate <= 0m)
            {
                return decimal.Round(
                    availableEmi * tenureMonths,
                    2,
                    MidpointRounding.AwayFromZero);
            }

            double growthDouble = Math.Pow(
                1d + (double)monthlyRate,
                tenureMonths);

            decimal growth = (decimal)growthDouble;

            decimal principal =
                availableEmi * (1m - (1m / growth)) / monthlyRate;

            return decimal.Round(
                Math.Max(0m, principal),
                2,
                MidpointRounding.AwayFromZero);
        }

        // Calculate existing EMI, maximum allowable EMI, available EMI,
        // and eligible loan principal for every tenure.
        private async Task<List<TenureEligibilityDto>>
            CalculateTenureOptionsAsync(
                int customerId,
                decimal monthlyIncome)
        {
            decimal existingMonthlyEmi =
                await GetExistingMonthlyEmiAsync(customerId);

            decimal maxAllowableEmi = Math.Max(
                0m,
                monthlyIncome * MaximumFoirPercent / 100m);

            decimal availableEmi = Math.Max(
                0m,
                maxAllowableEmi - existingMonthlyEmi);

            DateTime calculatedAt = DateTime.Now;

            return TenurePlans
                .Select(plan => new TenureEligibilityDto
                {
                    EligibilityTenureResultId = 0,
                    TenureMonths = plan.TenureMonths,
                    InterestRate = plan.InterestRate,
                    ExistingMonthlyEmi =
                        decimal.Round(existingMonthlyEmi, 2),
                    MaxAllowableEmi =
                        decimal.Round(maxAllowableEmi, 2),
                    AvailableEmi =
                        decimal.Round(availableEmi, 2),
                    EligibleLoanAmount = CalculateEligiblePrincipal(
                        availableEmi,
                        plan.InterestRate,
                        plan.TenureMonths),
                    CalculatedAt = calculatedAt
                })
                .ToList();
        }

        // Use the highest calculated tenure amount as the main LoanAmount.
        private static decimal GetMaximumTenureLoanAmount(
            IEnumerable<TenureEligibilityDto> options)
        {
            return options.Any()
                ? options.Max(x => x.EligibleLoanAmount)
                : 0m;
        }

        // Replace saved tenure options safely and return persisted values.
        private async Task<List<TenureEligibilityDto>>
            ReplaceTenureOptionsAsync(
                int eligibilityId,
                List<TenureEligibilityDto> options)
        {
            // Detach previously tracked tenure entities for this eligibility.
            // This avoids EF Core attempting to delete stale tracked rows.
            var trackedEntries = context.ChangeTracker
                .Entries<EligibilityTenureResult>()
                .Where(x => x.Entity.EligibilityId == eligibilityId)
                .ToList();

            foreach (var entry in trackedEntries)
            {
                entry.State = EntityState.Detached;
            }

            // Delete existing rows directly from the database.
            await context.EligibilityTenureResults
                .Where(x => x.EligibilityId == eligibilityId)
                .ExecuteDeleteAsync();

            // Insert the newly calculated tenure options.
            foreach (var option in options)
            {
                context.EligibilityTenureResults.Add(
                    new EligibilityTenureResult
                    {
                        EligibilityId = eligibilityId,
                        InterestRate = option.InterestRate,
                        TenureMonths = option.TenureMonths,
                        ExistingMonthlyEmi =
                            option.ExistingMonthlyEmi,
                        MaxAllowableEmi =
                            option.MaxAllowableEmi,
                        AvailableEmi =
                            option.AvailableEmi,
                        EligibleLoanAmount =
                            option.EligibleLoanAmount,
                        CalculatedAt = option.CalculatedAt
                    });
            }

            if (options.Count > 0)
            {
                await context.SaveChangesAsync();
            }

            return await GetSavedTenureOptionsAsync(eligibilityId);
        }

        // Fetch previously saved tenure calculations.
        private async Task<List<TenureEligibilityDto>>
            GetSavedTenureOptionsAsync(int eligibilityId)
        {
            return await context.EligibilityTenureResults
                .AsNoTracking()
                .Where(x => x.EligibilityId == eligibilityId)
                .OrderBy(x => x.TenureMonths)
                .Select(x => new TenureEligibilityDto
                {
                    EligibilityTenureResultId =
                        x.EligibilityTenureResultId,
                    TenureMonths = x.TenureMonths,
                    InterestRate = x.InterestRate,
                    ExistingMonthlyEmi = x.ExistingMonthlyEmi,
                    MaxAllowableEmi = x.MaxAllowableEmi,
                    AvailableEmi = x.AvailableEmi,
                    EligibleLoanAmount = x.EligibleLoanAmount,
                    CalculatedAt = x.CalculatedAt
                })
                .ToListAsync();
        }

        // Populate the EMI summary and tenure options in the response DTO.
        private static void SetCalculationSummary(
            EligibilityEvaluationDto result,
            List<TenureEligibilityDto> options)
        {
            result.TenureOptions = options
                .OrderBy(x => x.TenureMonths)
                .ToList();

            var firstOption = result.TenureOptions.FirstOrDefault();

            if (firstOption == null)
            {
                result.ExistingMonthlyEmi = 0m;
                result.MaxAllowableEmi = 0m;
                result.AvailableEmi = 0m;
                result.InterestRate = 0m;
                return;
            }

            // These values are shared across tenure options.
            result.ExistingMonthlyEmi =
                firstOption.ExistingMonthlyEmi;

            result.MaxAllowableEmi =
                firstOption.MaxAllowableEmi;

            result.AvailableEmi =
                firstOption.AvailableEmi;

            // The summary loan amount uses the option with the
            // highest eligible amount, so use that option's rate too.
            var highestLoanOption = result.TenureOptions
                .OrderByDescending(x => x.EligibleLoanAmount)
                .First();

            result.InterestRate = highestLoanOption.InterestRate;
        }

        // Automatic decision rules:
        // Score >= 800: Auto Approved
        // Score 650-799: Manual Review
        // Score < 650: Auto Rejected
        public async Task<int> ProcessAutomaticEligibilityAsync()
        {
            var reports = await GetLatestReportsAsync();

            var savedResults = await context.EligibilityResults
                .OrderByDescending(e => e.EligibilityId)
                .ToListAsync();

            var resultByCustomer = savedResults
                .GroupBy(e => e.CustomerId)
                .ToDictionary(g => g.Key, g => g.First());

            int automaticDecisionsProcessed = 0;

            foreach (var report in reports)
            {
                int customerId = report.CustomerId;

                resultByCustomer.TryGetValue(
                    customerId,
                    out var saved);

                // Preserve existing final decisions.
                // Pending and Manual Review records can be re-evaluated.
                if (saved != null && IsFinalDecision(saved.IsEligible))
                {
                    continue;
                }

                int score = report.CibilScore;

                decimal income = ParseIncome(
                    report.Customer!.MonthlyIncome);

                if (saved == null)
                {
                    saved = new EligibilityResult
                    {
                        CustomerId = customerId
                    };

                    context.EligibilityResults.Add(saved);
                    resultByCustomer[customerId] = saved;
                }

                saved.CibilScore = score;
                saved.RejectionReason = null;

                if (score >= AutomaticApprovalMinimumScore)
                {
                    var options = await CalculateTenureOptionsAsync(
                        customerId,
                        income);

                    saved.IsEligible = "Auto Approved";
                    saved.LoanAmount =
                        GetMaximumTenureLoanAmount(options);

                    await context.SaveChangesAsync();

                    await ReplaceTenureOptionsAsync(
                        saved.EligibilityId,
                        options);

                    automaticDecisionsProcessed++;
                }
                else if (score < ManualReviewMinimumScore)
                {
                    saved.IsEligible = "Auto Rejected";
                    saved.LoanAmount = 0m;
                    saved.RejectionReason =
                        "CIBIL score below 650.";

                    await context.SaveChangesAsync();

                    await ReplaceTenureOptionsAsync(
                        saved.EligibilityId,
                        new List<TenureEligibilityDto>());

                    automaticDecisionsProcessed++;
                }
                else
                {
                    // Scores 650-799 remain for Officer review.
                    var options = await CalculateTenureOptionsAsync(
                        customerId,
                        income);

                    saved.IsEligible = "Manual Review";
                    saved.LoanAmount = 0m;

                    await context.SaveChangesAsync();

                    await ReplaceTenureOptionsAsync(
                        saved.EligibilityId,
                        options);
                }
            }

            return automaticDecisionsProcessed;
        }

        // Return customers whose scores are 650-799 and whose decisions
        // are still pending or waiting for Officer review.
        public async Task<List<EligibilityCustomerDto>>
            GetManualReviewCustomersAsync()
        {
            var reports = await GetLatestReportsAsync();

            var savedResults = await context.EligibilityResults
                .AsNoTracking()
                .OrderByDescending(e => e.EligibilityId)
                .ToListAsync();

            var resultByCustomer = savedResults
                .GroupBy(e => e.CustomerId)
                .ToDictionary(g => g.Key, g => g.First());

            return reports
                .Where(r =>
                {
                    if (r.CibilScore < ManualReviewMinimumScore ||
                        r.CibilScore >= AutomaticApprovalMinimumScore)
                    {
                        return false;
                    }

                    if (!resultByCustomer.TryGetValue(
                        r.CustomerId,
                        out var saved))
                    {
                        return true;
                    }

                    return IsReviewableStatus(saved.IsEligible);
                })
                .Select(r => new EligibilityCustomerDto
                {
                    CustomerId = r.CustomerId,
                    CustomerName =
                        r.Customer!.FirstName + " " +
                        r.Customer.LastName,
                    Email = r.Customer.Email,
                    MonthlyIncome = ParseIncome(
                        r.Customer.MonthlyIncome),
                    CibilScore = r.CibilScore,
                    ApplicationStatus = "Manual Review"
                })
                .OrderBy(c => c.CustomerName)
                .ToList();
        }

        // Preview a Manual Review customer before an Officer decision.
        // This method does not save the Officer's final decision.
        public async Task<EligibilityEvaluationDto?> EvaluateAsync(
            int customerId)
        {
            var customer = await context.Customers
                .FirstOrDefaultAsync(
                    c => c.CustomerId == customerId);

            if (customer == null)
            {
                return null;
            }

            var report = await context.CibilReports
                .Where(r => r.CustomerId == customerId)
                .OrderByDescending(r => r.CheckDate)
                .ThenByDescending(r => r.CibilReportId)
                .FirstOrDefaultAsync();

            if (report == null ||
                report.CibilScore < ManualReviewMinimumScore ||
                report.CibilScore >= AutomaticApprovalMinimumScore)
            {
                return null;
            }

            var saved = await context.EligibilityResults
                .OrderByDescending(e => e.EligibilityId)
                .FirstOrDefaultAsync(e => e.CustomerId == customerId);

            if (saved != null &&
                !IsReviewableStatus(saved.IsEligible))
            {
                return null;
            }

            decimal income = ParseIncome(customer.MonthlyIncome);

            var options = await CalculateTenureOptionsAsync(
                customerId,
                income);

            var result = new EligibilityEvaluationDto
            {
                EligibilityId = saved?.EligibilityId ?? 0,
                CustomerId = customerId,
                CustomerName =
                    customer.FirstName + " " + customer.LastName,
                PanNumber = customer.Pan,
                CibilScore = report.CibilScore,
                ApplicationStatus = "Manual Review",
                MonthlyIncome = income,
                LoanAmount = GetMaximumTenureLoanAmount(options)
            };

            SetCalculationSummary(result, options);

            return result;
        }

        // Officer approves a Manual Review application.
        public async Task<EligibilityEvaluationDto?> ApproveAsync(
            int customerId)
        {
            var preview = await EvaluateAsync(customerId);

            if (preview == null)
            {
                return null;
            }

            var saved = await context.EligibilityResults
                .OrderByDescending(e => e.EligibilityId)
                .FirstOrDefaultAsync(e => e.CustomerId == customerId);

            if (saved == null)
            {
                saved = new EligibilityResult
                {
                    CustomerId = customerId
                };

                context.EligibilityResults.Add(saved);
            }

            saved.CibilScore = preview.CibilScore;
            saved.IsEligible = "Approved";
            saved.LoanAmount =
                GetMaximumTenureLoanAmount(preview.TenureOptions);
            saved.RejectionReason = null;

            await context.SaveChangesAsync();

            var savedOptions = await ReplaceTenureOptionsAsync(
                saved.EligibilityId,
                preview.TenureOptions);

            preview.EligibilityId = saved.EligibilityId;
            preview.ApplicationStatus = "Approved";
            preview.LoanAmount = saved.LoanAmount;
            preview.RejectionReason = null;

            SetCalculationSummary(preview, savedOptions);

            return preview;
        }

        // Officer rejects a Manual Review application.
        public async Task<EligibilityEvaluationDto?> RejectAsync(
            int customerId,
            string rejectionReason)
        {
            if (string.IsNullOrWhiteSpace(rejectionReason))
            {
                return null;
            }

            var preview = await EvaluateAsync(customerId);

            if (preview == null)
            {
                return null;
            }

            var saved = await context.EligibilityResults
                .OrderByDescending(e => e.EligibilityId)
                .FirstOrDefaultAsync(e => e.CustomerId == customerId);

            if (saved == null)
            {
                saved = new EligibilityResult
                {
                    CustomerId = customerId
                };

                context.EligibilityResults.Add(saved);
            }

            saved.CibilScore = preview.CibilScore;
            saved.IsEligible = "Rejected";
            saved.LoanAmount = 0m;
            saved.RejectionReason = rejectionReason.Trim();

            await context.SaveChangesAsync();

            await ReplaceTenureOptionsAsync(
                saved.EligibilityId,
                new List<TenureEligibilityDto>());

            preview.EligibilityId = saved.EligibilityId;
            preview.ApplicationStatus = "Rejected";
            preview.LoanAmount = 0m;
            preview.RejectionReason = saved.RejectionReason;
            preview.TenureOptions =
                new List<TenureEligibilityDto>();

            return preview;
        }

        // Retrieve the saved eligibility result for the logged-in Customer.
        public async Task<EligibilityEvaluationDto?> GetCustomerResultAsync(
            int userId)
        {
            var user = await context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.UserId == userId);

            if (user == null || user.CustomerId == null)
            {
                return null;
            }

            int customerId = user.CustomerId.Value;

            var customer = await context.Customers
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.CustomerId == customerId);

            var saved = await context.EligibilityResults
                .AsNoTracking()
                .Where(e => e.CustomerId == customerId)
                .OrderByDescending(e => e.EligibilityId)
                .FirstOrDefaultAsync();

            if (customer == null || saved == null)
            {
                return null;
            }

            string status;

            if (string.Equals(
                saved.IsEligible,
                "Auto Approved",
                StringComparison.OrdinalIgnoreCase))
            {
                status = "Auto Approved";
            }
            else if (string.Equals(
                saved.IsEligible,
                "Auto Rejected",
                StringComparison.OrdinalIgnoreCase))
            {
                status = "Auto Rejected";
            }
            else if (string.Equals(
                saved.IsEligible,
                "Yes",
                StringComparison.OrdinalIgnoreCase) ||
                string.Equals(
                    saved.IsEligible,
                    "Approved",
                    StringComparison.OrdinalIgnoreCase))
            {
                status = "Approved";
            }
            else if (string.Equals(
                saved.IsEligible,
                "No",
                StringComparison.OrdinalIgnoreCase) ||
                string.Equals(
                    saved.IsEligible,
                    "Rejected",
                    StringComparison.OrdinalIgnoreCase))
            {
                status = "Rejected";
            }
            else if (string.Equals(
                saved.IsEligible,
                "Manual Review",
                StringComparison.OrdinalIgnoreCase))
            {
                status = "Manual Review";
            }
            else
            {
                status = "Pending";
            }

            var tenureOptions = await GetSavedTenureOptionsAsync(
                saved.EligibilityId);

            var result = new EligibilityEvaluationDto
            {
                EligibilityId = saved.EligibilityId,
                CustomerId = customerId,
                CustomerName =
                    customer.FirstName + " " + customer.LastName,
                PanNumber = customer.Pan,
                CibilScore = saved.CibilScore,
                ApplicationStatus = status,
                LoanAmount = saved.LoanAmount,
                RejectionReason = saved.RejectionReason,
                MonthlyIncome = ParseIncome(customer.MonthlyIncome)
            };

            SetCalculationSummary(result, tenureOptions);

            return result;
        }
    }
}