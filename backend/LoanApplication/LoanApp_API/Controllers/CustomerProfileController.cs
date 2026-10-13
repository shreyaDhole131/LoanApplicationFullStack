using LoanApp_API.Application.DTO;
using LoanApp_API.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LoanApp_API.Controllers
{
    [ApiController]
    [Route("api/customer-profile")]
    [Authorize]
    public class CustomerProfileController : ControllerBase
    {
        private readonly ICustomerProfileRepository _repo;

        public CustomerProfileController(ICustomerProfileRepository repo)
        {
            _repo = repo;
        }

        [HttpGet]
        public IActionResult GetProfile()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userIdClaim == null)
                return Unauthorized();

            int userId = int.Parse(userIdClaim);

            var profile = _repo.GetCustomerProfile(userId);

            if (profile == null)
            {
                return NotFound(new
                {
                    message = "Customer profile not found"
                });
            }

            return Ok(profile);
        }

        [HttpPut]
        public IActionResult UpdateProfile(UpdateCustomerProfileDTO model)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userIdClaim == null)
                return Unauthorized();

            int userId = int.Parse(userIdClaim);

            var profile = _repo.UpdateCustomerProfile(userId, model);

            if (profile == null)
            {
                return NotFound(new
                {
                    message = "Customer profile not found"
                });
            }

            return Ok(new
            {
                message = "Customer profile updated successfully",
                profile = profile
            });
        }
    }
}