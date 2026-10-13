using LoanApp_API.Application.Common;
using LoanApp_API.Application.DTO;
using LoanApp_API.Application.DTO.User;
using LoanApp_API.Application.Interfaces;
using LoanApp_API.Domain.Entities;
using LoanApp_API.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace LoanApp_API.Infrastructure.Repositories
{
    public class UserService : IUserService
    {
        private readonly AppDbContext _context;
        private readonly IMemoryCache _cache;

        public UserService(AppDbContext context, IMemoryCache cache)
        {
            _context = context;
            _cache = cache;
        }

        public async Task<ApiResponse<UserResponseDto>> CreateUserAsync(CreateUserDto dto)
        {
            var user = new User
            {
                RoleId = dto.RoleId,
                CustomerId = dto.CustomerId,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                Mobile = dto.Mobile,
                Password = dto.Password
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            _cache.Remove("users");

            var response = new UserResponseDto
            {
                UserId = user.UserId,
                RoleId = user.RoleId,
                CustomerId = user.CustomerId,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Mobile = user.Mobile
            };

            return new ApiResponse<UserResponseDto>
            {
                Success = true,
                Message = "User created successfully",
                Data = response
            };
        }

        public async Task<ApiResponse<List<UserResponseDto>>> GetUsersAsync(PaginationDto pagination)
        {
            var users = await _cache.GetOrCreateAsync(
                "users",
                async entry =>
                {
                    entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5);

                    return await _context.Users
                        .OrderBy(x => x.UserId)
                        .Select(user => new UserResponseDto
                        {
                            UserId = user.UserId,
                            RoleId = user.RoleId,
                            CustomerId = user.CustomerId,
                            FirstName = user.FirstName,
                            LastName = user.LastName,
                            Email = user.Email,
                            Mobile = user.Mobile
                        })
                        .ToListAsync();
                });

            var query = users!.AsEnumerable();

            if (pagination.LastId.HasValue)
            {
                query = query.Where(x => x.UserId > pagination.LastId.Value);
            }
            else
            {
                query = query.Skip(
                    (pagination.Page - 1) * pagination.PageSize
                );
            }

            var response = query
                .Take(pagination.PageSize)
                .ToList();

            return new ApiResponse<List<UserResponseDto>>
            {
                Success = true,
                Message = "Users fetched successfully",
                Data = response
            };
        }
    }
}