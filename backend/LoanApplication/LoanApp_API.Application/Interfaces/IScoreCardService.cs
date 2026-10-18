using LoanApp_API.Application.DTO.ScoreCard;

namespace LoanApp_API.Application.Interfaces
{
    public interface IScoreCardService
    {
        Task<List<ScoreCardCustomerDto>> GetCustomersAsync();

        Task<ScoreCardDto?> GenerateScoreCardAsync(int customerId);
    }
}