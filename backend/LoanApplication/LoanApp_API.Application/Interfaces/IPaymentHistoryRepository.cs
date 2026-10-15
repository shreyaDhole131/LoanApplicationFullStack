
using LoanApp_API.Application.DTO.PaymentHistory;

namespace LoanApp_API.Application.Interfaces
{
    public interface IPaymentHistoryRepository
    {
        Task<List<PaymentHistoryDto>> GetPaymentHistory(
            int customerId,
            int? loanAccountId,
            int months);
    }
}
