
using LoanApp_API.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LoanApp_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class NotificationServiceController : ControllerBase
    {
        private readonly INotificationService service;

        public NotificationServiceController(
            INotificationService service)
        {
            this.service = service;
        }


        [HttpGet("customer")]
        public IActionResult GetCustomerNotifications()
        {
            var customerIdClaim = User.FindFirst("CustomerId");

            if (customerIdClaim == null ||
                !int.TryParse(customerIdClaim.Value, out int customerId))
            {
                return Unauthorized("CustomerId claim is missing or invalid.");
            }

            var notifications =
                service.SendCustomerNotifications(customerId);

            return Ok(notifications);
        }

        [HttpGet("unread-count")]
        public IActionResult GetUnreadCount()
        {
            var customerIdClaim = User.FindFirst("CustomerId");

            if (customerIdClaim == null ||
                !int.TryParse(customerIdClaim.Value, out int customerId))
            {
                return Unauthorized("CustomerId claim is missing or invalid.");
            }

            var count = service.FetchUnreadCount(customerId);

            return Ok(count);
        }

        [HttpPut("mark-read/{notificationId}")]
        public async Task<IActionResult> MarkAsRead(int notificationId)
        {
            var customerIdClaim = User.FindFirst("CustomerId");

            if (customerIdClaim == null ||
                !int.TryParse(customerIdClaim.Value, out int customerId))
            {
                return Unauthorized("CustomerId claim is missing or invalid.");
            }

            var result =
                await service.MarkAsRead(notificationId, customerId);

            if (!result)
                return NotFound("Notification not found.");

            return Ok(new
            {
                message = "Notification marked as read."
            });
        }
    }
}
