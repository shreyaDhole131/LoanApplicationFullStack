using LoanApp_API.Application.DTO.Cibil;
using LoanApp_API.Application.Interfaces;
using LoanApp_API.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LoanApp_API.Infrastructure.Repositories
{
    public class CibilService : ICibilService
    {
        private readonly AppDbContext context;

        public CibilService(AppDbContext context)
        {
            this.context = context;
        }

        // Generate CIBIL demo score using the specified scoring rules
        public async Task<CibilScoreResponseDto?> GenerateScoreAsync(
            int customerId)
        {
            var customer = await context.Customers
                .FirstOrDefaultAsync(c => c.CustomerId == customerId);

            if (customer == null)
                return null;

            if (!decimal.TryParse(
                    customer.MonthlyIncome,
                    out decimal income)
                || income <= 0)
            {
                throw new InvalidOperationException(
                    "Valid monthly income is required to calculate the score.");
            }

            // Start at zero because the rules define points for each factor.
            int score = 0;

            // 1. Income score
            if (income < 25000)
                score += 100;
            else if (income < 50000)
                score += 200;
            else if (income <= 100000)
                score += 300;
            else
                score += 400;

            // 2. Employment score
            string employmentType =
                customer.EmploymentType?.Trim() ?? string.Empty;

            if (string.Equals(
                employmentType,
                "Government",
                StringComparison.OrdinalIgnoreCase))
            {
                score += 200;
            }
            else if (
                string.Equals(
                    employmentType,
                    "Private",
                    StringComparison.OrdinalIgnoreCase)
                ||
                string.Equals(
                    employmentType,
                    "Salaried",
                    StringComparison.OrdinalIgnoreCase))
            {
                // Treat Salaried as Private employment.
                score += 150;
            }
            else if (string.Equals(
                employmentType,
                "Self-employed",
                StringComparison.OrdinalIgnoreCase))
            {
                score += 100;
            }

            // 3. Age score
            if (customer.Age >= 21 && customer.Age <= 24)
                score += 50;
            else if (customer.Age >= 25 && customer.Age <= 45)
                score += 150;
            else if (customer.Age >= 46 && customer.Age <= 60)
                score += 100;

            // 4. Calculate existing fixed monthly obligations.
            // Each LoanAccount is counted once.
            var loanAccounts = await context.LoanAccounts
                .AsNoTracking()
                .Where(l => l.CustomerId == customerId)
                .Select(l => new
                {
                    l.LoanStatus,
                    l.EmiAmount
                })
                .ToListAsync();

            // Exclude closed loans and invalid/non-positive EMI values.
            decimal totalFixedMonthlyObligations = loanAccounts
                .Where(l => !string.Equals(
                    l.LoanStatus?.Trim(),
                    "Closed",
                    StringComparison.OrdinalIgnoreCase))
                .Where(l => l.EmiAmount > 0)
                .Sum(l => l.EmiAmount);

            // 5. Calculate FOIR.
            decimal grossMonthlyIncome = income;

            decimal foir =
                totalFixedMonthlyObligations
                / grossMonthlyIncome * 100m;

            // Customers exceeding 40% FOIR receive no FOIR points.
            const decimal maxFoirPercent = 40m;

            bool isWithinFoirLimit = foir <= maxFoirPercent;

            if (isWithinFoirLimit)
            {
                if (foir < 30m)
                    score += 250;
                else
                    score += 150;
            }

            // Keep the score within the 300-900 range.
            score = Math.Clamp(score, 300, 900);

            // Determine the score status after all points are applied.
            string status = GetScoreStatus(score);

            // 6. Find or create the customer's CIBIL report.
            var report = await context.CibilReports
                .FirstOrDefaultAsync(r => r.CustomerId == customerId);

            if (report == null)
            {
                report = new LoanApp_API.Domain.Entities.CibilReport
                {
                    CustomerId = customerId
                };

                context.CibilReports.Add(report);
            }

            // 7. Save the generated score.
            report.PanNo = customer.Pan;
            report.CibilScore = score;
            report.CheckDate = DateTime.Now;

            await context.SaveChangesAsync();

            // Return DTO with masked PAN.
            return new CibilScoreResponseDto
            {
                CustomerId = customer.CustomerId,
                CustomerName =
                    customer.FirstName + " " + customer.LastName,
                PanNo = MaskPan(report.PanNo),
                CibilScore = report.CibilScore,
                ScoreStatus = status,
                CheckDate = report.CheckDate
            };
        }

        // Get saved CIBIL report by customer ID.
        public async Task<CibilScoreResponseDto?> GetCibilReportAsync(
            int customerId)
        {
            var report = await context.CibilReports
                .AsNoTracking()
                .Include(r => r.Customer)
                .FirstOrDefaultAsync(r => r.CustomerId == customerId);

            if (report == null || report.Customer == null)
                return null;

            return new CibilScoreResponseDto
            {
                CustomerId = report.CustomerId,
                CustomerName =
                    report.Customer.FirstName + " " +
                    report.Customer.LastName,
                PanNo = MaskPan(report.PanNo),
                CibilScore = report.CibilScore,
                ScoreStatus = GetScoreStatus(report.CibilScore),
                CheckDate = report.CheckDate
            };
        }

        // Get all saved CIBIL reports.
        public async Task<List<CibilScoreResponseDto>>
            GetAllCibilReportsAsync()
        {
            var reports = await context.CibilReports
                .AsNoTracking()
                .Include(r => r.Customer)
                .Where(r => r.Customer != null)
                .ToListAsync();

            return reports.Select(r => new CibilScoreResponseDto
            {
                CustomerId = r.CustomerId,
                CustomerName =
                    r.Customer!.FirstName + " " + r.Customer.LastName,
                PanNo = MaskPan(r.PanNo),
                CibilScore = r.CibilScore,
                ScoreStatus = GetScoreStatus(r.CibilScore),
                CheckDate = r.CheckDate
            }).ToList();
        }

        // Get customers for the Angular frontend.
        public async Task<List<CibilCustomerDto>> GetCustomersAsync()
        {
            return await context.Customers
                .AsNoTracking()
                .Select(c => new CibilCustomerDto
                {
                    CustomerId = c.CustomerId,
                    Name = c.FirstName + " " + c.LastName,
                    Email = c.Email,
                    MonthlyIncome = c.MonthlyIncome,

                    MaskedPan = string.IsNullOrEmpty(c.Pan)
                        ? string.Empty
                        : c.Pan.Length > 6
                            ? c.Pan.Substring(0, 5) +
                              new string('*', c.Pan.Length - 6) +
                              c.Pan.Substring(c.Pan.Length - 1)
                            : new string('*', c.Pan.Length)
                })
                .ToListAsync();
        }

        // Score status based on the specified ranges.
        private string GetScoreStatus(int score)
        {
            if (score >= 900)
                return "Excellent";
            else if (score >= 800)
                return "Very Good";
            else if (score >= 750)
                return "Good";
            else if (score >= 700)
                return "Average";
            else if (score >= 650)
                return "Risky";
            else
                return "Reject";
        }

        // Mask PAN before returning it to the frontend.
        private string MaskPan(string? pan)
        {
            if (string.IsNullOrWhiteSpace(pan))
                return string.Empty;

            pan = pan.Trim();

            if (pan.Length <= 6)
                return new string('*', pan.Length);

            return pan.Substring(0, 5) +
                   new string('*', pan.Length - 6) +
                   pan.Substring(pan.Length - 1);
        }
    }
}