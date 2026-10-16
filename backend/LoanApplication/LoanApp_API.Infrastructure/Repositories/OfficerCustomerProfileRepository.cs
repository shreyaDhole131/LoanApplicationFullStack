using AutoMapper;
using LoanApp_API.Application.DTO;
using LoanApp_API.Application.Interfaces;
using LoanApp_API.Infrastructure.Data;

namespace LoanApp_API.Infrastructure.Repositories
{
    public class OfficerCustomerProfileRepository : IOfficerCustomerProfileRepository
    {
        private readonly AppDbContext _db;
        private readonly IMapper _mapper;

        public OfficerCustomerProfileRepository(AppDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        public List<OfficerCustomerListVM> GetAllCustomers()
        {
            var customers = _db.Customers.ToList();
            var result = new List<OfficerCustomerListVM>();

            foreach (var customer in customers)
            {
                var customerData = _mapper.Map<OfficerCustomerListVM>(customer);

                var cibil = _db.CibilReports.FirstOrDefault(x => x.CustomerId == customer.CustomerId);

                if (cibil != null)
                {
                    customerData.CibilScore = cibil.CibilScore;
                }

                var hasKyc = _db.KycDocuments.Any(x => x.CustomerId == customer.CustomerId);

                if (hasKyc)
                {
                    customerData.KycStatus = "Done";
                }
                else
                {
                    customerData.KycStatus = "Pending";
                }

                var loan = _db.LoanAccounts
                    .Where(x => x.CustomerId == customer.CustomerId)
                    .OrderByDescending(x => x.CreatedAt)
                    .FirstOrDefault();

                if (loan != null)
                {
                    customerData.LoanAmount = loan.LoanAmount;
                    customerData.LoanStatus = loan.LoanStatus;
                    customerData.Date = loan.DisbursementDate;
                }
                else
                {
                    customerData.LoanAmount = 0;
                    customerData.LoanStatus = "-";
                    customerData.Date = null;
                }

                result.Add(customerData);
            }

            return result;
        }

        public OfficerCustomerProfileVM? GetCustomerProfile(int customerId)
        {
            var customer = _db.Customers.FirstOrDefault(x => x.CustomerId == customerId);

            if (customer == null)
            {
                return null;
            }

            var profile = _mapper.Map<OfficerCustomerProfileVM>(customer);

            var cibil = _db.CibilReports.FirstOrDefault(x => x.CustomerId == customerId);

            if (cibil != null)
            {
                profile.CibilScore = cibil.CibilScore;
            }

            var scoreCard = _db.ScoreCards.FirstOrDefault(x => x.CustomerId == customerId);

            if (scoreCard != null)
            {
                profile.RiskCategory = scoreCard.RiskCategory;
                profile.EligibleLoanAmount = scoreCard.EligibleLoanAmount;
            }

            var loanAccounts = _db.LoanAccounts
                .Where(x => x.CustomerId == customerId)
                .ToList();

            foreach (var loan in loanAccounts)
            {
                var loanDetails = _mapper.Map<OfficerLoanDetailsVM>(loan);

                var emiHistory = _db.EmiSchedules
                    .Where(x => x.LoanAccountId == loan.LoanAccountId)
                    .OrderBy(x => x.InstallmentNo)
                    .ToList();

                loanDetails.EmiHistory = _mapper.Map<List<OfficerEmiHistoryVM>>(emiHistory);

                profile.Loans.Add(loanDetails);
            }

            return profile;
        }
    }
}