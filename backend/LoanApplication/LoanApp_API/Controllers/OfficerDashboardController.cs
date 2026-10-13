using LoanApp_API.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LoanApp_API.Controllers
{
    [ApiController]
    [Route("api/officer-dashboard")]
    [Authorize(Roles = "2")]
    public class OfficerDashboardController : ControllerBase
    {
        private readonly IOfficerDashboardRepository _repo;

        public OfficerDashboardController(
            IOfficerDashboardRepository repo)
        {
            _repo = repo;
        }

        [HttpGet]
        public IActionResult GetDashboard()
        {
            var dashboard = _repo.GetOfficerDashboard();

            return Ok(dashboard);
        }
    }
}