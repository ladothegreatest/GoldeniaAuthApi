using System.Net;
using System.Net.Mail;

namespace GoldeniaAuthApi.Services
{
    public interface IEmailService
    {
        Task SendVerificationCodeAsync(string email, string code);
    }

    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendVerificationCodeAsync(string email, string code)
        {
            try
            {
                var smtpClient = new SmtpClient(_configuration["Email:Host"])
                {
                    Port = int.Parse(_configuration["Email:Port"]),
                    Credentials = new NetworkCredential(
                        _configuration["Email:Username"],
                        _configuration["Email:Password"]
                    ),
                    EnableSsl = true,
                };

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(_configuration["Email:From"], "Goldenia KYC"),
                    Subject = "Email Verification Code - Goldenia",
                    Body = $@"
                        <h2>Welcome to Goldenia!</h2>
                        <p>Your verification code is: <strong>{code}</strong></p>
                        <p>This code will expire in 15 minutes.</p>
                        <p>If you didn't request this, please ignore this email.</p>
                    ",
                    IsBodyHtml = true,
                };

                mailMessage.To.Add(email);
                await smtpClient.SendMailAsync(mailMessage);
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to send email: {ex.Message}");
            }
        }
    }
}
