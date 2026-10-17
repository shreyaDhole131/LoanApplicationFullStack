using AutoMapper;
using LoanApp_API.Application.DTO;
using LoanApp_API.Application.Interfaces;
using LoanApp_API.Infrastructure.Data;

namespace LoanApp_API.Infrastructure.Repositories
{
    public class CustomerDashboardRepository : ICustomerDashboardRepository
    {
        private readonly AppDbContext _db;
        private readonly IMapper _mapper;

        public CustomerDashboardRepository(
            AppDbContext db,
            IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }


        public CustomerDashboardVM? GetCustomerDashboard(int userId)
        {
            // =====================================================
            // 1. FIND LOGGED-IN USER
            // =====================================================

            var user = _db.Users
                .FirstOrDefault(x => x.UserId == userId);

            if (user == null || user.CustomerId == null)
                return null;


            // =====================================================
            // 2. GET CUSTOMER ID
            // =====================================================

            int customerId = user.CustomerId.Value;


            // =====================================================
            // 3. GET PARTICULAR CUSTOMER
            // =====================================================

            var customer = _db.Customers
                .FirstOrDefault(x => x.CustomerId == customerId);

            if (customer == null)
                return null;


            // =====================================================
            // 4. GET ONLY THIS CUSTOMER'S LOAN DEALS
            // =====================================================

            var deals = _db.LoanDeals
                .Where(x => x.CustomerId == customerId)
                .ToList();


            // =====================================================
            // 5. GET ONLY THIS CUSTOMER'S LOAN ACCOUNTS
            // =====================================================

            var loanAccounts = _db.LoanAccounts
                .Where(x => x.CustomerId == customerId)
                .ToList();


            // =====================================================
            // 6. CREATE DASHBOARD
            // =====================================================

            var dashboard = new CustomerDashboardVM
            {
                CustomerId = customer.CustomerId,

                CustomerName =
                    customer.FirstName + " " + customer.LastName,

                TotalApplications = deals.Count,

                TotalActiveLoanAccounts =
                    loanAccounts.Count(
                        x => x.LoanStatus == "Active"
                    ),

                Applications =
                    _mapper.Map<List<CustomerApplicationVM>>(
                        deals
                    ),

                LoanAccounts =
                    _mapper.Map<List<CustomerLoanDropdownVM>>(
                        loanAccounts
                    ),

                KycDocuments =
                    _mapper.Map<List<CustomerKycVM>>(
                        _db.KycDocuments
                            .Where(x => x.CustomerId == customerId)
                            .ToList()
                    )
            };


            // =====================================================
            // 7. IF CUSTOMER HAS NO LOAN
            // =====================================================

            if (loanAccounts.Count == 0)
            {
                return dashboard;
            }


            // =====================================================
            // 8. SELECT FIRST LOAN
            // =====================================================

            var selectedLoan = loanAccounts.First();


            dashboard.SelectedLoanAccountNo =
                selectedLoan.LoanAccountNo;


            // =====================================================
            // 9. GET EMI SCHEDULE
            // =====================================================

            var emiSchedules = _db.EmiSchedules
                .Where(x =>
                    x.LoanAccountId ==
                    selectedLoan.LoanAccountId
                )
                .ToList();


            // =====================================================
            // 10. PAID EMI
            // =====================================================

            var paidEmis = emiSchedules
                .Where(x =>
                    x.PaymentStatus == "Paid"
                )
                .ToList();


            // =====================================================
            // 11. NEXT EMI
            // =====================================================

            var nextEmi = emiSchedules
                .Where(x =>
                    x.PaymentStatus != "Paid" &&
                    x.DueDate >= DateTime.Now
                )
                .OrderBy(x => x.DueDate)
                .FirstOrDefault();


            // =====================================================
            // 12. MAP LOAN ACCOUNT
            // =====================================================

            var loanDetails =
                _mapper.Map<CustomerLoanDetailsVM>(
                    selectedLoan
                );


            // =====================================================
            // 13. ADD CALCULATED EMI INFORMATION
            // =====================================================

            loanDetails.NextEmiDate =
                nextEmi?.DueDate;

            loanDetails.TotalEmis =
                emiSchedules.Count;

            loanDetails.PaidEmis =
                paidEmis.Count;

            loanDetails.RemainingEmis =
                emiSchedules.Count -
                paidEmis.Count;


            dashboard.SelectedLoanDetails =
                loanDetails;


            // =====================================================
            // 14. EMI SUMMARY
            // =====================================================

            dashboard.EmiSummary =
                new CustomerEmiSummaryVM
                {
                    TotalEmis =
                        emiSchedules.Count,

                    PaidEmis =
                        paidEmis.Count,

                    RemainingEmis =
                        emiSchedules.Count -
                        paidEmis.Count,

                    TotalPaidAmount =
                        selectedLoan.TotalPaidAmount,

                    NextEmiDate =
                        nextEmi?.DueDate,

                    NextEmiAmount =
                        nextEmi?.Emi
                };


            // =====================================================
            // 15. RECENT PAYMENTS
            // =====================================================

            dashboard.RecentPayments =
                _mapper.Map<List<CustomerEmiPaymentVM>>(
                    paidEmis
                        .OrderByDescending(x => x.PaidDate)
                        .Take(5)
                        .ToList()
                );


            // =====================================================
            // 16. RETURN CUSTOMER DASHBOARD
            // =====================================================

            return dashboard;
        }
    }
}