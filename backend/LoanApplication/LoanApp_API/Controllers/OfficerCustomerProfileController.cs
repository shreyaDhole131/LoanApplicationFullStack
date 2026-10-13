using LoanApp_API.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LoanApp_API.Controllers
{
    [ApiController]
    [Route("api/officer/customer-profile")]
    [Authorize(Roles = "2")]
    public class OfficerCustomerProfileController : ControllerBase
    {
        private readonly IOfficerCustomerProfileRepository _repo;

        public OfficerCustomerProfileController(
            IOfficerCustomerProfileRepository repo)
        {
            _repo = repo;
        }

        [HttpGet("{customerId}")]
        public IActionResult GetCustomerProfile(int customerId)
        {
            var profile = _repo.GetCustomerProfile(customerId);

            if (profile == null)
            {
                return NotFound(new
                {
                    message = "Customer not found"
                });
            }

            return Ok(profile);
        }
    }
}