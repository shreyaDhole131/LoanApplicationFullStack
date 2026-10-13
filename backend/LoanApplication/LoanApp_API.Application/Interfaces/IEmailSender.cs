namespace LoanApp_API.Application.Interfaces
{
    public interface IEmailSender
    {
        Task SendOtpAsync(string email, string otp);
    }
}