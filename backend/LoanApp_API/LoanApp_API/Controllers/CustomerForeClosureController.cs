using LoanApp_API.Application.DTO.ForeClosure;
using LoanApp_API.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace LoanApp_API.Controllers;

[ApiController]
[Route("api/customer/foreclosure")]
public class CustomerForeClosureController : ControllerBase
{
    private readonly IForeClosureRepository _repo;

    public CustomerForeClosureController(IForeClosureRepository repo)
    {
        _repo = repo;
    }

    // "Select Loan Account" dropdown
    [HttpGet("loans/{customerId:int}")]
    public async Task<IActionResult> GetLoans(int customerId)
        => Ok(await _repo.GetCustomerLoans(customerId));

    // 3 cards on top
    [HttpGet("summary/{loanAccountId:int}")]
    public async Task<IActionResult> GetSummary(int loanAccountId)
    {
        var data = await _repo.GetSummary(loanAccountId);
        return data == null ? NotFound(new { message = "Loan not found" }) : Ok(data);
    }

    // Submit Request button
    [HttpPost]
    public async Task<IActionResult> Submit(ForeClosureDto dto)
    {
        var (success, message) = await _repo.SubmitRequest(dto);
        return success ? Ok(new { message }) : BadRequest(new { message });
    }

    // "My Foreclosure Requests" table
    [HttpGet("requests/{customerId:int}")]
    public async Task<IActionResult> GetMyRequests(int customerId)
        => Ok(await _repo.GetMyRequests(customerId));
}