using LoanApp_API.Application.DTO;
using LoanApp_API.Application.DTO.User;
using LoanApp_API.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace LoanApp_API.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    [EnableRateLimiting("fixed")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser(CreateUserDto dto)
        {
            var result = await _userService.CreateUserAsync(dto);

            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetUsers([FromQuery] PaginationDto pagination)
        {
            var result = await _userService.GetUsersAsync(pagination);

            return Ok(result);
        }
    }
}