using LoanApp_API.Application.DTO;
using LoanApp_API.Application.DTO.Cibil;

namespace LoanApp_API.Application.Interfaces
{
    public interface ICibilService
    {
        Task<List<CibilCustomerDto>> GetCustomersAsync();

        Task<CibilScoreResponseDto?> GenerateScoreAsync(int customerId);

        Task<CibilScoreResponseDto?> GetCibilReportAsync(int customerId);

        Task<List<CibilScoreResponseDto>> GetAllCibilReportsAsync();
    }
}