using LoanApp_API.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace LoanApp_API.Controllers;

[ApiController]
[Route("api/officer/foreclosure")]
public class OfficerForeClosureController : ControllerBase
{
    private readonly IForeClosureRepository _repo;

    public OfficerForeClosureController(IForeClosureRepository repo)
    {
        _repo = repo;
    }

    // Dashboard table
    [HttpGet]
    public async Task<IActionResult> GetAll()
        => Ok(await _repo.GetAllRequests());

    // View page
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetDetails(int id)
    {
        var data = await _repo.GetRequestById(id);
        return data == null ? NotFound(new { message = "Request not found" }) : Ok(data);
    }

    [HttpPut("{id:int}/approve")]
    public async Task<IActionResult> Approve(int id)
    {
        var (success, message) = await _repo.Approve(id);
        return success ? Ok(new { message }) : BadRequest(new { message });
    }

    [HttpPut("{id:int}/reject")]
    public async Task<IActionResult> Reject(int id)
    {
        var (success, message) = await _repo.Reject(id);
        return success ? Ok(new { message }) : BadRequest(new { message });
    }
}