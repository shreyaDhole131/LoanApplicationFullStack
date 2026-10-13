
using LoanApp_API.Application.DTO.Eligibility;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LoanApp_API.Application.Interfaces
{
    public interface IEligibilityService
    {
        Task<List<EligibilityCustomerDto>> GetManualReviewCustomersAsync();

        Task<EligibilityEvaluationDto?> EvaluateAndSaveAsync(int customerId);

        Task<EligibilityEvaluationDto?> GetSavedResultAsync(int customerId);
    }
}
