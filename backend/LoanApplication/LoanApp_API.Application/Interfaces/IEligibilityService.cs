using LoanApp_API.Application.DTO.Eligibility;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LoanApp_API.Application.Interfaces
{
    public interface IEligibilityService
    {
        Task<int> ProcessAutomaticEligibilityAsync();

        Task<List<EligibilityCustomerDto>>
            GetManualReviewCustomersAsync();

        Task<EligibilityEvaluationDto?>
            EvaluateAsync(int customerId);

        Task<EligibilityEvaluationDto?>
            ApproveAsync(int customerId);

        Task<EligibilityEvaluationDto?>
            RejectAsync(int customerId, string rejectionReason);

        Task<EligibilityEvaluationDto?>
            GetCustomerResultAsync(int userId);
    }
}