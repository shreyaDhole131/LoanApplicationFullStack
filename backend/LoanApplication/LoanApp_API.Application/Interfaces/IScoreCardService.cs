
using LoanApp_API.Application.DTO.ScoreCard;
using LoanApp_API.Domain.Entities;

namespace LoanApp_API.Application.Interfaces
{
    public interface IScoreCardService
    {
        Task<List<ScoreCardCustomerDto>> GetCustomersAsync();

        Task<ScoreCard?> GenerateScoreCardAsync(int customerId);

        Task<ScoreCard?> GetScoreCardAsync(int customerId);

        Task<List<ScoreCard>> GetAllScoreCardsAsync();
    }
}
