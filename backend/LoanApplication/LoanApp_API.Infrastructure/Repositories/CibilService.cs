using LoanApp_API.Application.DTO.Cibil;
using LoanApp_API.Application.Interfaces;
using LoanApp_API.Domain.Entities;
using LoanApp_API.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LoanApp_API.Infrastructure.Repositories
{
    public class CibilService : ICibilService
    {
        private readonly AppDbContext context;

        public CibilService(AppDbContext context)
        {
            this.context = context;
        }

        // Generate CIBIL Score
        public async Task<CibilScoreResponseDto?> GenerateScoreAsync(int customerId)
        {
            var customer = await context.Customers
                .FirstOrDefaultAsync(c => c.CustomerId == customerId);

            if (customer == null)
                return null;

            decimal.TryParse(customer.MonthlyIncome, out decimal income);

            int score = 500;

            // Income points
            if (income < 25000)
                score += 100;
            else if (income < 50000)
                score += 200;
            else if (income <= 100000)
                score += 300;
            else
                score += 400;

            // Employment points
            if (customer.EmploymentType == "Government")
                score += 200;
            else if (customer.EmploymentType == "Private")
                score += 150;
            else if (customer.EmploymentType == "Self-employed")
                score += 100;

            // Age points
            if (customer.Age >= 21 && customer.Age <= 24)
                score += 50;
            else if (customer.Age >= 25 && customer.Age <= 45)
                score += 150;
            else if (customer.Age >= 46 && customer.Age <= 60)
                score += 100;

            // Calculate FOIR
            var loans = await context.LoanAccounts
                .Where(l => l.CustomerId == customerId &&
                            l.LoanStatus != "Closed")
                .ToListAsync();

            decimal totalEmi = loans.Sum(l => l.EmiAmount);

            decimal foir = income > 0
                ? totalEmi / income * 100
                : 100;

            if (foir < 30)
                score += 250;
            else if (foir <= 50)
                score += 150;
            else if (foir <= 60)
                score += 75;

            // Limit score
            score = Math.Clamp(score, 300, 900);

            // Get status
            string status = GetScoreStatus(score);

            // Find existing report
            var report = await context.CibilReports
                .FirstOrDefaultAsync(r => r.CustomerId == customerId);

            if (report == null)
            {
                report = new CibilReport
                {
                    CustomerId = customerId
                };

                context.CibilReports.Add(report);
            }

            // Save report
            report.PanNo = customer.Pan;
            report.CibilScore = score;
            report.CheckDate = DateTime.Now;

            await context.SaveChangesAsync();

            return new CibilScoreResponseDto
            {
                CustomerId = customer.CustomerId,
                CustomerName = customer.FirstName + " " + customer.LastName,
                PanNo = report.PanNo,
                CibilScore = report.CibilScore,
                ScoreStatus = status,
                CheckDate = report.CheckDate
            };
        }

        // Get CIBIL Report by Customer ID
        public async Task<CibilScoreResponseDto?> GetCibilReportAsync(int customerId)
        {
            var report = await context.CibilReports
                .Include(r => r.Customer)
                .FirstOrDefaultAsync(r => r.CustomerId == customerId);

            if (report == null || report.Customer == null)
                return null;

            return new CibilScoreResponseDto
            {
                CustomerId = report.Customer.CustomerId,
                CustomerName = report.Customer.FirstName + " " +
                               report.Customer.LastName,
                PanNo = report.PanNo,
                CibilScore = report.CibilScore,
                ScoreStatus = GetScoreStatus(report.CibilScore),
                CheckDate = report.CheckDate
            };
        }

        // Get All CIBIL Reports
        public async Task<List<CibilScoreResponseDto>> GetAllCibilReportsAsync()
        {
            var reports = await context.CibilReports
                .Include(r => r.Customer)
                .ToListAsync();

            return reports
                .Where(r => r.Customer != null)
                .Select(r => new CibilScoreResponseDto
                {
                    CustomerId = r.Customer!.CustomerId,
                    CustomerName = r.Customer.FirstName + " " +
                                   r.Customer.LastName,
                    PanNo = r.PanNo,
                    CibilScore = r.CibilScore,
                    ScoreStatus = GetScoreStatus(r.CibilScore),
                    CheckDate = r.CheckDate
                })
                .ToList();
        }

        // Common method for score status
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

        public async Task<List<CibilCustomerDto>> GetCustomersAsync()
        {
            return await context.Customers
                .Select(c => new CibilCustomerDto
                {
                    CustomerId = c.CustomerId,
                    Name = c.FirstName + " " + c.LastName,
                    Email = c.Email,
                    MonthlyIncome = c.MonthlyIncome
                })
                .ToListAsync();
        }
    }
}
