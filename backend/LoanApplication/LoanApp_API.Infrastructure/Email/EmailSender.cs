using LoanApp_API.Application.Interfaces;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using MimeKit;

namespace LoanApp_API.Infrastructure.Email
{
    public class EmailSender : IEmailSender
    {
        private readonly IConfiguration _configuration;

        public EmailSender(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendOtpAsync(string email, string otp)
        {
            var senderName = _configuration["Email:SenderName"];
            var senderEmail = _configuration["Email:SenderEmail"];
            var appPassword = _configuration["Email:AppPassword"];
            var smtpHost = _configuration["Email:SmtpHost"];
            var smtpPort = _configuration["Email:SmtpPort"];

            if (string.IsNullOrWhiteSpace(senderName) ||
                string.IsNullOrWhiteSpace(senderEmail) ||
                string.IsNullOrWhiteSpace(appPassword) ||
                string.IsNullOrWhiteSpace(smtpHost) ||
                !int.TryParse(smtpPort, out var port))
            {
                throw new InvalidOperationException(
                    "Email configuration is missing or invalid.");
            }

            var message = new MimeMessage();

            message.From.Add(
                new MailboxAddress(senderName, senderEmail));

            message.To.Add(MailboxAddress.Parse(email));

            message.Subject = "LoanApp - Email Verification OTP";

            message.Body = new TextPart("plain")
            {
                Text =
                    $"Hello,\n\n" +
                    $"Your LoanApp verification OTP is: {otp}\n\n" +
                    "This OTP is valid for 5 minutes.\n\n" +
                    "Please do not share this OTP with anyone.\n\n" +
                    "Thank you,\nLoanApp Team"
            };

            using var smtp = new SmtpClient();

            await smtp.ConnectAsync(
                smtpHost,
                port,
                SecureSocketOptions.StartTls);

            await smtp.AuthenticateAsync(
                senderEmail,
                appPassword);

            await smtp.SendAsync(message);

            await smtp.DisconnectAsync(true);
        }
    }
}