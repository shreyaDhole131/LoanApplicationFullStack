using LoanApp_API.Application.DTO;
using LoanApp_API.Application.Interfaces;
using LoanApp_API.Infrastructure.Data;

namespace LoanApp_API.Infrastructure.Repositories
{
    public class OfficerCustomerProfileRepository : IOfficerCustomerProfileRepository
    {
        private readonly AppDbContext _db;

        public OfficerCustomerProfileRepository(AppDbContext db)
        {
            _db = db;
        }

        public OfficerCustomerProfileVM? GetCustomerProfile(int customerId)
        {
            // 1. Get Customer
            var customer = _db.Customers
                .FirstOrDefault(x => x.CustomerId == customerId);

            if (customer == null)
                return null;

            // 2. Customer Details
            var profile = new OfficerCustomerProfileVM
            {
                CustomerId = customer.CustomerId,
                Name = customer.FirstName + " " + customer.LastName,
                Email = customer.Email,
                Mobile = customer.MobileNo,
                Pan = customer.Pan,
                Aadhaar = customer.AadhaarNo,
                Salary = customer.MonthlyIncome,
                Employment = customer.EmploymentType,
                Age = customer.Age
            };

            // 3. Get CIBIL Score
            var cibil = _db.CibilReports
                .FirstOrDefault(x => x.CustomerId == customerId);

            if (cibil != null)
            {
                profile.CibilScore = cibil.CibilScore;
            }

            // 4. Get Score Card
            var scoreCard = _db.ScoreCards
                .FirstOrDefault(x => x.CustomerId == customerId);

            if (scoreCard != null)
            {
                profile.RiskCategory = scoreCard.RiskCategory;
                profile.EligibleLoanAmount = scoreCard.EligibleLoanAmount;
            }

            // 5. Get Loan Accounts
            var loanAccounts = _db.LoanAccounts
                .Where(x => x.CustomerId == customerId)
                .ToList();

            foreach (var loan in loanAccounts)
            {
                // 6. Create Loan Details
                var loanDetails = new OfficerLoanDetailsVM
                {
                    LoanAccountNo = loan.LoanAccountNo,
                    LoanAmount = loan.LoanAmount,
                    EmiAmount = loan.EmiAmount,
                    InterestRate = loan.InterestRate,
                    LoanStatus = loan.LoanStatus
                };

                // 7. Get EMI History
                var emiHistory = _db.EmiSchedules
                    .Where(x => x.LoanAccountId == loan.LoanAccountId)
                    .OrderBy(x => x.InstallmentNo)
                    .ToList();

                foreach (var emi in emiHistory)
                {
                    loanDetails.EmiHistory.Add(
                        new OfficerEmiHistoryVM
                        {
                            InstallmentNo = emi.InstallmentNo,
                            DueDate = emi.DueDate,
                            Emi = emi.Emi,
                            PaymentStatus = emi.PaymentStatus,
                            PaidDate = emi.PaidDate
                        }
                    );
                }

                // 8. Add Loan to Customer Profile
                profile.Loans.Add(loanDetails);
            }

            // 9. Return Complete Profile
            return profile;
        }
    }
}