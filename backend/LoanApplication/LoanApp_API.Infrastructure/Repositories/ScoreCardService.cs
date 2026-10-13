
using LoanApp_API.Application.DTO.ScoreCard;
using LoanApp_API.Application.Interfaces;
using LoanApp_API.Domain.Entities;
using LoanApp_API.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LoanApp_API.Infrastructure.Repositories
{
    public class ScoreCardService : IScoreCardService
    {
        private readonly AppDbContext context;

        public ScoreCardService(AppDbContext context)
        {
            this.context = context;
        }

        // Get all customers for the customer list
        public async Task<List<ScoreCardCustomerDto>> GetCustomersAsync()
        {
            return await context.Customers
                .Select(c => new ScoreCardCustomerDto
                {
                    CustomerId = c.CustomerId,
                    Name = c.FirstName + " " + c.LastName,
                    Email = c.Email,
                    MonthlyIncome = c.MonthlyIncome
                })
                .ToListAsync();
        }

        // Generate a new scorecard or update the existing one
        public async Task<ScoreCard?> GenerateScoreCardAsync(int customerId)
        {
            var customer = await context.Customers
                .FirstOrDefaultAsync(c => c.CustomerId == customerId);

            if (customer == null)
                return null;

            var report = await context.CibilReports
                .FirstOrDefaultAsync(r => r.CustomerId == customerId);

            if (report == null)
                return null;

            int cibilScore = report.CibilScore;

            // Calculate risk category
            string riskCategory;

            if (cibilScore >= 750)
                riskCategory = "Low";
            else if (cibilScore >= 650)
                riskCategory = "Medium";
            else
                riskCategory = "High";

            // Calculate eligible loan amount
            decimal.TryParse(customer.MonthlyIncome, out decimal income);

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

            scoreCard.RiskCategory = riskCategory;
            scoreCard.EligibleLoanAmount = loanAmount;

            await context.SaveChangesAsync();

            return scoreCard;
        }

        // Retrieve an existing scorecard without recalculating it
        public async Task<ScoreCard?> GetScoreCardAsync(int customerId)
        {
            return await context.ScoreCards
                .FirstOrDefaultAsync(s => s.CustomerId == customerId);
        }

        // Get all generated scorecards
        public async Task<List<ScoreCard>> GetAllScoreCardsAsync()
        {
            return await context.ScoreCards
                .OrderBy(s => s.CustomerId)
                .ToListAsync();
        }
    }
}
