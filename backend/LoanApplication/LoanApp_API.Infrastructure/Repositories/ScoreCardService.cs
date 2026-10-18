using LoanApp_API.Application.DTO.ScoreCard;
using LoanApp_API.Application.Interfaces;
using LoanApp_API.Domain.Entities;
using LoanApp_API.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace LoanApp_API.Infrastructure.Repositories
{
    public class ScoreCardService : IScoreCardService
    {
        private readonly AppDbContext context;

        public ScoreCardService(AppDbContext context)
        {
            this.context = context;
        }

        // Get customers for the officer's customer list
        public async Task<List<ScoreCardCustomerDto>> GetCustomersAsync()
        {
            return await context.Customers
                .Select(c => new ScoreCardCustomerDto
                {
                    CustomerId = c.CustomerId,
                    Name = c.FirstName + " " + c.LastName,
                    Email = c.Email,
                    MonthlyIncome = c.MonthlyIncome ?? string.Empty
                })
                .ToListAsync();
        }

        // Generate or update the scorecard
        public async Task<ScoreCardDto?> GenerateScoreCardAsync(
            int customerId)
        {
            var customer = await context.Customers
                .FirstOrDefaultAsync(c => c.CustomerId == customerId);

            if (customer == null)
                return null;

            var report = await context.CibilReports
                .FirstOrDefaultAsync(r => r.CustomerId == customerId);

            if (report == null)
                return null;

            // Calculate risk category
            int cibilScore = report.CibilScore;

            string riskCategory;

            if (cibilScore >= 750)
                riskCategory = "Low";
            else if (cibilScore >= 650)
                riskCategory = "Medium";
            else
                riskCategory = "High";

            // Read monthly income
            decimal income;

            if (!decimal.TryParse(
                customer.MonthlyIncome,
                NumberStyles.Any,
                CultureInfo.InvariantCulture,
                out income))
            {
                decimal.TryParse(
                    customer.MonthlyIncome,
                    NumberStyles.Any,
                    CultureInfo.CurrentCulture,
                    out income);
            }

            // Calculate eligible loan amount
            decimal loanAmount;

            if (income > 100000)
                loanAmount = 5000000;
            else if (income > 50000)
                loanAmount = 4000000;
            else
                loanAmount = 0;

            // Find existing scorecard
            var scoreCard = await context.ScoreCards
                .FirstOrDefaultAsync(s => s.CustomerId == customerId);

            if (scoreCard == null)
            {
                scoreCard = new ScoreCard
                {
                    CustomerId = customerId
                };

                context.ScoreCards.Add(scoreCard);
            }

            // Insert or update calculated values
            scoreCard.RiskCategory = riskCategory;
            scoreCard.EligibleLoanAmount = loanAmount;

            await context.SaveChangesAsync();

            return new ScoreCardDto
            {
                ScoreCardId = scoreCard.ScoreCardId,
                CustomerId = scoreCard.CustomerId,
                RiskCategory = scoreCard.RiskCategory,
                EligibleLoanAmount = scoreCard.EligibleLoanAmount
            };
        }
    }
}