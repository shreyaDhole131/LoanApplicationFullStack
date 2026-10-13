using LoanApp_API.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LoanApp_API.Controllers
{
    [ApiController]
    [Route("api/customer-dashboard")]
    [Authorize]
    public class CustomerDashboardController : ControllerBase
    {
        private readonly ICustomerDashboardRepository _repo;

        public CustomerDashboardController(ICustomerDashboardRepository repo)
        {
            _repo = repo;
        }

        [HttpGet]
        public IActionResult GetDashboard()
        {
            var userIdClaim =User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userIdClaim == null)
                return Unauthorized();

            int userId = int.Parse(userIdClaim);

            var dashboard =_repo.GetCustomerDashboard(userId);

            if (dashboard == null)
            {
                return NotFound(new
                {
                    message = "Customer dashboard not found"
                });
            }

            return Ok(dashboard);
        }
    }
}