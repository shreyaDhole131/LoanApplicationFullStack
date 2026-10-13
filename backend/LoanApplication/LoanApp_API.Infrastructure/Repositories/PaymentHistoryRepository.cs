
using LoanApp_API.Application.DTO.ForeClosure;
using LoanApp_API.Application.DTO.PaymentHistory;
using LoanApp_API.Application.Interfaces;
using LoanApp_API.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LoanApp_API.Infrastructure.Repositories
{
    
    public class PaymentHistoryRepository : IPaymentHistoryRepository
    {
        private readonly AppDbContext _db;

        public PaymentHistoryRepository(AppDbContext db)
        {
            _db = db;
        }

        //all loans of this customer (Active and Closed)
        public async Task<List<CustomerLoanDto>> GetCustomerLoans(int customerId)
        {
            return await _db.LoanAccounts
                .Where(x => x.CustomerId == customerId)
                .Select(x => new CustomerLoanDto
                {
                    LoanAccountId = x.LoanAccountId,
                    LoanAccountNo = x.LoanAccountNo
                })
                .ToListAsync();
        }

        public async Task<List<PaymentHistoryDto>> GetPaymentHistory(int customerId, int? loanAccountId, int months)
        {
            // only this customer's payments
            var query = _db.LoanPayment
                .Where(x => x.LoanAccount!.CustomerId == customerId);

            // filter 1: selected loan
            if (loanAccountId != null)
                query = query.Where(x => x.LoanAccountId == loanAccountId);

            // filter 2: Last 1 / 3 / 6 months (0 = all)
            if (months > 0)
            {
                DateTime fromDate = DateTime.Now.AddMonths(-months);
                query = query.Where(x => x.PaymentDate >= fromDate);
            }

            return await query
                .OrderByDescending(x => x.PaymentDate)
                .Select(x => new PaymentHistoryDto
                {
                    PaymentId = x.PaymentId,
                    PaymentDate = x.PaymentDate,
                    LoanAccountNo = x.LoanAccount!.LoanAccountNo,
                    PaidAmount = x.PaidAmount,
                    PaymentMode = x.PaymentMode,
                    TransactionReference = x.TransactionReference,
                    PaymentStatus = x.PaymentStatus
                })
                .ToListAsync();
        }
    }
}
