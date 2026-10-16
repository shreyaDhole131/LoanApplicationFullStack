
namespace LoanApp_API.Application.Interfaces
{
    public interface ITotpService
    {
        string GenerateSecret();

        string GenerateOtpUri(string email, string secret);

        bool VerifyOtp(string secret, string otp);
    }
}
