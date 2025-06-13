using MailKit.Net.Smtp;
using MimeKit;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

namespace Authservice.API
{
    public class EmailService
    {
        private readonly IConfiguration _config;
        public EmailService(IConfiguration config) => _config = config;

        public async Task SendVerificationEmailAsync(string toEmail, string verificationToken)
        {
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse(Environment.GetEnvironmentVariable("EMAIL_FROM")));
            email.To.Add(MailboxAddress.Parse(toEmail));
            email.Subject = "Verify your email";
            var baseUrl = Environment.GetEnvironmentVariable("EMAIL_VERIFICATION_BASE_URL");
            var verifyUrl = $"{baseUrl}/{verificationToken}";
            email.Body = new TextPart("plain")
            {
                Text = $"Please verify your email by clicking this link: {verifyUrl}"
            };

            using var smtp = new SmtpClient();
            await smtp.ConnectAsync(
                Environment.GetEnvironmentVariable("EMAIL_SMTP_SERVER"),
                int.Parse(Environment.GetEnvironmentVariable("EMAIL_SMTP_PORT")!),
                false
            );
            await smtp.AuthenticateAsync(
                Environment.GetEnvironmentVariable("EMAIL_SMTP_USER"),
                Environment.GetEnvironmentVariable("EMAIL_SMTP_PASS")
            );
            await smtp.SendAsync(email);
            await smtp.DisconnectAsync(true);
        }
    }
}