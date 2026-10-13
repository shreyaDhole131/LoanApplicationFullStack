
using LoanApp_API.Application.DTO.Common;
using LoanApp_API.Application.DTO.Cibil;
using LoanApp_API.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace LoanApp_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CibilController : ControllerBase
    {
        private readonly ICibilService service;

        public CibilController(ICibilService service)
        {
            this.service = service;
        }

        // Generate CIBIL Score
        [HttpPost]
        [Route("GenerateCibil/{customerId}")]
        public async Task<IActionResult> GenerateScore(int customerId)
        {
            if (customerId <= 0)
            {
                return BadRequest(new ResponseWrapper<CibilScoreResponseDto>
                {
                    Success = false,
                    Message = "Invalid customer ID.",
                    Data = null
                });
            }

            var data = await service.GenerateScoreAsync(customerId);

            if (data == null)
            {
                return NotFound(new ResponseWrapper<CibilScoreResponseDto>
                {
                    Success = false,
                    Message = "Customer not found.",
                    Data = null
                });
            }

            var response = new ResponseWrapper<CibilScoreResponseDto>
            {
                Success = true,
                Message = "CIBIL Score Generated Successfully",
                Data = data
            };

            return Ok(response);
        }

        // Get CIBIL Report by Customer ID
        [HttpGet]
        [Route("GetCibil/{customerId}")]
        public async Task<IActionResult> GetCibilReport(int customerId)
        {
            if (customerId <= 0)
            {
                return BadRequest(new ResponseWrapper<CibilScoreResponseDto>
                {
                    Success = false,
                    Message = "Invalid customer ID.",
                    Data = null
                });
            }

            var data = await service.GetCibilReportAsync(customerId);

            if (data == null)
            {
                return NotFound(new ResponseWrapper<CibilScoreResponseDto>
                {
                    Success = false,
                    Message = "CIBIL Report Not Found.",
                    Data = null
                });
            }

            var response = new ResponseWrapper<CibilScoreResponseDto>
            {
                Success = true,
                Message = "CIBIL Report Fetched Successfully",
                Data = data
            };

            return Ok(response);
        }

        // Get All CIBIL Reports
        [HttpGet]
        [Route("GetAllCibil")]
        public async Task<IActionResult> GetAllCibilReports()
        {
            var data = await service.GetAllCibilReportsAsync();

            var response = new ResponseWrapper<List<CibilScoreResponseDto>>
            {
                Success = true,
                Message = "CIBIL Reports Fetched Successfully",
                Data = data
            };

            return Ok(response);
        }

        //Get Customers
        [HttpGet]
        [Route("customers")]
        public async Task<IActionResult> GetCustomers()
        {
            var data = await service.GetCustomersAsync();

            return Ok(new ResponseWrapper<List<CibilCustomerDto>>
            {
                Success = true,
                Message = "Customers fetched successfully.",
                Data = data
            });
        }
    }
}
