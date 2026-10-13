using LoanApp_API.Application.DTO;
using LoanApp_API.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LoanApp_API.Controllers
{
    [ApiController]
    [Route("api/officer/kyc")]
    [Authorize(Roles = "2")]
    public class OfficerKycController : ControllerBase
    {
        private readonly IOfficerKycRepository _repo;

        public OfficerKycController(IOfficerKycRepository repo)
        {
            _repo = repo;
        }

        // 1. Get all customers having KYC documents
        [HttpGet]
        public IActionResult GetKycCustomers()
        {
            var customers = _repo.GetKycCustomers();

            return Ok(customers);
        }

        // 2. Get KYC details of a particular customer
        [HttpGet("{customerId}")]
        public IActionResult GetCustomerKyc(int customerId)
        {
            var customer = _repo.GetCustomerKyc(customerId);

            if (customer == null)
            {
                return NotFound(new
                {
                    message = "Customer not found"
                });
            }

            return Ok(customer);
        }

        // 3. Approve selected KYC documents
        [HttpPut("approve")]
        public IActionResult ApproveKyc(OfficerKycStatusDTO model)
        {
            if (model.DocumentIds == null || model.DocumentIds.Count == 0)
            {
                return BadRequest(new
                {
                    message = "Please select at least one document"
                });
            }

            var result = _repo.UpdateKycStatus(
                model.DocumentIds,
                "Approved"
            );

            if (!result)
            {
                return NotFound(new
                {
                    message = "KYC documents not found"
                });
            }

            return Ok(new
            {
                message = "KYC approved successfully"
            });
        }

        // 4. Reject selected KYC documents
        [HttpPut("reject")]
        public IActionResult RejectKyc(OfficerKycStatusDTO model)
        {
            if (model.DocumentIds == null || model.DocumentIds.Count == 0)
            {
                return BadRequest(new
                {
                    message = "Please select at least one document"
                });
            }

            var result = _repo.UpdateKycStatus(
                model.DocumentIds,
                "Rejected"
            );

            if (!result)
            {
                return NotFound(new
                {
                    message = "KYC documents not found"
                });
            }

            return Ok(new
            {
                message = "KYC rejected successfully"
            });
        }
    }
}