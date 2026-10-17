
using System.ComponentModel.DataAnnotations;

namespace LoanApp_API.Application.DTO
{
    public class VerifyOtpDTO
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [RegularExpression(@"^\d{6}$")]
        public string Otp { get; set; } = string.Empty;
    }
}
