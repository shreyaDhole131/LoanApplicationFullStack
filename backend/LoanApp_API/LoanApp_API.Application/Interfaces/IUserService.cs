using LoanApp_API.Application.Common;
using LoanApp_API.Application.DTO;
using LoanApp_API.Application.DTO.User;

namespace LoanApp_API.Application.Interfaces
{
    public interface IUserService
    {
        Task<ApiResponse<UserResponseDto>> CreateUserAsync(CreateUserDto dto);

        Task<ApiResponse<List<UserResponseDto>>> GetUsersAsync(PaginationDto pagination);
    }
}