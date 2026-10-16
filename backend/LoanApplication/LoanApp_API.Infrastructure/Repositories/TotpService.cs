using LoanApp_API.Application.Interfaces;
using OtpNet;

namespace LoanApp_API.Infrastructure.Services
{
    public class TotpService : ITotpService
    {
        public string GenerateSecret()
        {
            return Base32Encoding.ToString(
                KeyGeneration.GenerateRandomKey(20));
        }

        public string GenerateOtpUri(string email, string secret)
        {
            return $"otpauth://totp/LoanApp:{Uri.EscapeDataString(email)}" +
                   $"?secret={secret}" +
                   "&issuer=LoanApp&digits=6&period=30";
        }

        public bool VerifyOtp(string secret, string otp)
        {
            if (string.IsNullOrWhiteSpace(secret) ||
                string.IsNullOrWhiteSpace(otp) ||
                otp.Length != 6 ||
                !otp.All(char.IsDigit))
            {
                return false;
            }

            try
            {
                var key = Base32Encoding.ToBytes(secret);
                var totp = new Totp(key);

                return totp.VerifyTotp(
                    otp,
                    out _,
                    new VerificationWindow(1, 1));
            }
            catch (FormatException)
            {
                return false;
            }
        }
    }
}