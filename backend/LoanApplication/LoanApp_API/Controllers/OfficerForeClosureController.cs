
using LoanApp_API.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LoanApp_API.Controllers
{
    [ApiController]
    [Route("api/officer/foreclosure")]
    [Authorize]
    public class OfficerForeClosureController : ControllerBase
    {
        private readonly IForeClosureRepository _foreClosureRepository;

        public OfficerForeClosureController(
            IForeClosureRepository foreClosureRepository)
        {
            _foreClosureRepository = foreClosureRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllRequests()
        {
            var requests =
                await _foreClosureRepository.GetAllRequests();

            return Ok(requests);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetRequestById(int id)
        {
            var request =
                await _foreClosureRepository.GetRequestById(id);

            if (request == null)
                return NotFound(new { message = "Foreclosure request not found." });

            return Ok(request);
        }

       
        [HttpPut("{id}")]
        public async Task<IActionResult> ChangeStatus(
            int id,
            [FromBody] string status)
        {
            var (success, message) =
                await _foreClosureRepository.ChangeStatus(id, status);

            if (!success)
                return BadRequest(new { message });

            return Ok(new { message });
        }
    }
}
