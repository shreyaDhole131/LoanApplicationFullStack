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
            var message = new MimeMessage();

            message.From.Add(
                new MailboxAddress(
                    _configuration["Email:SenderName"],
                    _configuration["Email:SenderEmail"]
                )
            );

            message.To.Add(
                MailboxAddress.Parse(email)
            );

            message.Subject = "LoanApp - Email Verification OTP";

            message.Body = new TextPart("plain")
            {
                Text =
                    $"Hello,\n\n" +
                    $"Your LoanApp verification OTP is: {otp}\n\n" +
                    $"This OTP is valid for 5 minutes.\n\n" +
                    $"Please do not share this OTP with anyone.\n\n" +
                    $"Thank you,\n" +
                    $"LoanApp Team"
            };

            using var smtp = new SmtpClient();

            await smtp.ConnectAsync(
                _configuration["Email:SmtpHost"],
                int.Parse(_configuration["Email:SmtpPort"]!),
                SecureSocketOptions.StartTls
            );

            await smtp.AuthenticateAsync(
                _configuration["Email:SenderEmail"],
                _configuration["Email:AppPassword"]
            );

            await smtp.SendAsync(message);

            await smtp.DisconnectAsync(true);
        }
    }
}