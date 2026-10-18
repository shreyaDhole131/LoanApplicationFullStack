
using AutoMapper;
using AutoMapper.QueryableExtensions;
using LoanApp_API.Application.DTO.PaymentHistory;
using LoanApp_API.Application.Interfaces;
using LoanApp_API.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LoanApp_API.Infrastructure.Repositories
{
    public class PaymentHistoryRepository : IPaymentHistoryRepository
    {
        private readonly AppDbContext _db;
        private readonly IMapper _mapper;

        public PaymentHistoryRepository(AppDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        public async Task<List<PaymentHistoryDto>> GetPaymentHistory(
            int customerId, int? loanAccountId, int months)
        {
            var query = _db.LoanPayment
                .Where(x => x.LoanAccount!.CustomerId == customerId);

            if (loanAccountId != null)
            {
                query = query.Where(
                    x => x.LoanAccountId == loanAccountId);
            }

            if (months > 0)
            {
                DateTime fromDate = DateTime.Now.AddMonths(-months);

                query = query.Where(
                    x => x.PaymentDate >= fromDate);
            }

            return await query
                .OrderByDescending(x => x.PaymentDate)
                .ProjectTo<PaymentHistoryDto>(
                    _mapper.ConfigurationProvider)
                .ToListAsync();
        }
    }
}
