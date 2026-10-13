namespace LoanApp_API.Application.DTO
{
    public class VerifyOtpDTO
    {
        public string Email { get; set; } = string.Empty;

        public string Otp { get; set; } = string.Empty;
    }
}