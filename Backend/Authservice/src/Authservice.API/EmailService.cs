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
            email.From.Add(MailboxAddress.Parse(_config["EmailSettings:FromEmail"]));
            email.To.Add(MailboxAddress.Parse(toEmail));
            email.Subject = "Verify your email";
            var baseUrl = _config["EmailSettings:VerificationBaseUrl"];
            var verifyUrl = $"{baseUrl}/user/confirm/{verificationToken}";
            email.Body = new TextPart("plain")
            {
                Text = $"Please verify your email by clicking this link: {verifyUrl}"
            };

            using var smtp = new SmtpClient();
            await smtp.ConnectAsync(_config["EmailSettings:SmtpServer"], int.Parse(_config["EmailSettings:SmtpPort"]), false);
            await smtp.AuthenticateAsync(_config["EmailSettings:SmtpUser"], _config["EmailSettings:SmtpPass"]);
            await smtp.SendAsync(email);
            await smtp.DisconnectAsync(true);
        }
    }
}