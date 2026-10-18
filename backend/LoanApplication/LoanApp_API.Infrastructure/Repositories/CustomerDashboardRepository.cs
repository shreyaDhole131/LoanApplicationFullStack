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



            if (loanAccounts.Count == 0)
            {
                return dashboard;
            }



            var selectedLoan = loanAccounts.First();


            dashboard.SelectedLoanAccountNo =
                selectedLoan.LoanAccountNo;



            var emiSchedules = _db.EmiSchedules
                .Where(x =>
                    x.LoanAccountId ==
                    selectedLoan.LoanAccountId
                )
                .ToList();



            var paidEmis = emiSchedules
                .Where(x =>
                    x.PaymentStatus == "Paid"
                )
                .ToList();


            

            var nextEmi = emiSchedules
                .Where(x =>
                    x.PaymentStatus != "Paid" &&
                    x.DueDate >= DateTime.Now
                )
                .OrderBy(x => x.DueDate)
                .FirstOrDefault();


           

            var loanDetails =
                _mapper.Map<CustomerLoanDetailsVM>(
                    selectedLoan
                );


            

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


           

            dashboard.RecentPayments =
                _mapper.Map<List<CustomerEmiPaymentVM>>(
                    paidEmis
                        .OrderByDescending(x => x.PaidDate)
                        .Take(5)
                        .ToList()
                );


            

            return dashboard;
        }
    }
}