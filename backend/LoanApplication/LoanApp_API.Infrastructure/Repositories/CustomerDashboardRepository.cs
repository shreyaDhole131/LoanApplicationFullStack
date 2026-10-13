using LoanApp_API.Application.DTO;
using LoanApp_API.Application.Interfaces;
using LoanApp_API.Infrastructure.Data;

namespace LoanApp_API.Infrastructure.Repositories
{
    public class CustomerDashboardRepository : ICustomerDashboardRepository
    {
        private readonly AppDbContext _db;

        public CustomerDashboardRepository(AppDbContext db)
        {
            _db = db;
        }

        public CustomerDashboardVM? GetCustomerDashboard(int userId)
        {
            var user = _db.Users
                .FirstOrDefault(x => x.UserId == userId);

            if (user == null || user.CustomerId == null)
                return null;

            int customerId = user.CustomerId.Value;

            var customer = _db.Customers
                .FirstOrDefault(x => x.CustomerId == customerId);

            if (customer == null)
                return null;

            var deals = _db.LoanDeals
                .Where(x => x.CustomerId == customerId)
                .ToList();

            var loanAccounts = _db.LoanAccounts
                .Where(x => x.CustomerId == customerId)
                .ToList();

            var dashboard = new CustomerDashboardVM
            {
                TotalApplications = deals.Count,

                TotalActiveLoanAccounts = loanAccounts
                    .Count(x => x.LoanStatus == "Active"),

                Applications = deals
                    .Select(x => new CustomerApplicationVM
                    {
                        DealId = x.DealId,
                        LoanType = x.LoanType,
                        LoanAmount = x.LoanAmount,
                        ApprovedAmount = x.ApprovedAmount,
                        CurrentStatus = x.CurrentStatus,
                        AppliedDate = x.AppliedDate
                    })
                    .ToList(),

                LoanAccounts = loanAccounts
                    .Select(x => new CustomerLoanDropdownVM
                    {
                        LoanAccountNo = x.LoanAccountNo
                    })
                    .ToList(),

                KycDocuments = _db.KycDocuments
                    .Where(x => x.CustomerId == customerId)
                    .Select(x => new CustomerKycVM
                    {
                        DocumentId = x.DocumentId,
                        DocumentType = x.DocumentType,
                        VerificationStatus = x.VerificationStatus
                    })
                    .ToList()
            };

            if (loanAccounts.Count == 0)
                return dashboard;

            var selectedLoan = loanAccounts.FirstOrDefault();

            if (selectedLoan == null)
                return dashboard;

            dashboard.SelectedLoanAccountNo =
                selectedLoan.LoanAccountNo;

            var emiSchedules = _db.EmiSchedules
                .Where(x => x.LoanAccountId == selectedLoan.LoanAccountId)
                .ToList();

            var paidEmis = emiSchedules
                .Where(x => x.PaymentStatus == "Paid")
                .ToList();

            var nextEmi = emiSchedules
                .Where(x =>
                    x.PaymentStatus != "Paid" &&
                    x.DueDate >= DateTime.Now)
                .OrderBy(x => x.DueDate)
                .FirstOrDefault();

            dashboard.SelectedLoanDetails = new CustomerLoanDetailsVM
            {
                LoanAccountNo = selectedLoan.LoanAccountNo,

                LoanType = deals
                    .Where(x => x.DealId == selectedLoan.DealId)
                    .Select(x => x.LoanType)
                    .FirstOrDefault() ?? string.Empty,

                LoanAmount = selectedLoan.LoanAmount,

                OutstandingPrincipal =
                    selectedLoan.OutstandingPrincipal,

                LoanStatus =
                    selectedLoan.LoanStatus,

                InterestRate =
                    selectedLoan.InterestRate,

                TenureMonths =
                    selectedLoan.TenureMonths,

                EmiAmount =
                    selectedLoan.EmiAmount,

                DisbursementDate =selectedLoan.DisbursementDate,

                TotalPaidAmount =selectedLoan.TotalPaidAmount,

                NextEmiDate =nextEmi?.DueDate,

                TotalEmis =emiSchedules.Count,

                PaidEmis =paidEmis.Count,

                RemainingEmis =emiSchedules.Count - paidEmis.Count
            };

            dashboard.EmiSummary = new CustomerEmiSummaryVM
            {
                TotalEmis =emiSchedules.Count,

                PaidEmis =paidEmis.Count,

                RemainingEmis =emiSchedules.Count - paidEmis.Count,

                TotalPaidAmount =selectedLoan.TotalPaidAmount,

                NextEmiDate =nextEmi?.DueDate,

                NextEmiAmount =nextEmi?.Emi
            };

            dashboard.RecentPayments = paidEmis
                .OrderByDescending(x => x.PaidDate)
                .Take(5)
                .Select(x => new CustomerEmiPaymentVM
                {
                    InstallmentNo =x.InstallmentNo,

                    DueDate =x.DueDate,

                    Emi =x.Emi,

                    PaymentStatus =x.PaymentStatus,

                    PaidDate =x.PaidDate
                })
                .ToList();

            return dashboard;
        }
    }
}