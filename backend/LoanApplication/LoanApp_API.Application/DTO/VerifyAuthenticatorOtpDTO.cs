
using System.ComponentModel.DataAnnotations;

namespace LoanApp_API.Application.DTO
{
    public class VerifyAuthenticatorOtpDTO
    {
        [Required]
        [RegularExpression(@"^\d{6}$")]
        public string Otp { get; set; } = string.Empty;
    }
}
