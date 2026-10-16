using LoanApp_API.Application.DTO.ForeClosure;

namespace LoanApp_API.Application.Interfaces
{
    public interface IForeClosureRepository
    {
        // Customer
        Task<List<CustomerLoanDto>> GetCustomerLoans(int customerId);

        Task<ForeClosureSummaryDto?> GetSummary(int loanAccountId);

        Task<(bool Success, string Message)> SubmitRequest(
            ForeClosureDto dto);

        Task<List<CustomerForeClosureDto>> GetMyRequests(
            int customerId);

        // Customer payment (Razorpay)
        Task<(bool Success, string Message, ForeClosureOrderDto? Data)> CreatePaymentOrder(
            int requestId, int customerId);

        Task<(bool Success, string Message)> VerifyPayment(
            VerifyForeClosurePaymentDto dto, int customerId);

        // Officer
        Task<List<OfficerForeClosureDto>> GetAllRequests();

        Task<OfficerForeClosureDto?> GetRequestById(int id);

        Task<(bool Success, string Message)> ChangeStatus(
            int id, string status);
    }
}