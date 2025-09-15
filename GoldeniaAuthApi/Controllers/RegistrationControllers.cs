using Microsoft.AspNetCore.Mvc;
using GoldeniaAuthApi.Services;
using GoldeniaAuthApi.Data;
using GoldeniaAuthApi.Models;

namespace GoldeniaKycApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RegistrationController : ControllerBase
    {
        private readonly IEmailService _emailService;

        public RegistrationController(IEmailService emailService)
        {
            _emailService = emailService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegistrationRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Creates the user
            var user = new User
            {
                Id = new Random().Next(1000, 9999),
                Email = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName,
                PhoneNumber = request.PhoneNumber,
                IsEmailVerified = false,
                CreatedAt = DateTime.UtcNow
            };

            // Generates the email verification code here, so it's sent while registering
            var verificationCode = new Random().Next(100000, 999999).ToString();

            // Stored verification code in datastore
            DataStore.EmailVerifications.Add(new EmailVerification
            {
                Id = new Random().Next(1, 1000),
                Email = request.Email,
                VerificationCode = verificationCode,
                IsVerified = false,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddMinutes(15)
            });

            try
            {
                await _emailService.SendVerificationCodeAsync(request.Email, verificationCode);

                return Ok(new
                {
                    Message = "User registered successfully. Verification code sent to your email.",
                    User = user
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Registration successful, but failed to send email." });
            }
        }

        [HttpPost("verify-email")]
        public IActionResult VerifyEmail([FromBody] VerifyEmailRequest request)
        {
            var verification = DataStore.EmailVerifications
                .FirstOrDefault(v => v.Email == request.Email && v.VerificationCode == request.Code);

            if (verification == null)
            {
                return BadRequest(new { Message = "Invalid verification code" });
            }

            if (verification.ExpiresAt < DateTime.UtcNow)
            {
                return BadRequest(new { Message = "Verification code expired" });
            }

            if (verification.IsVerified)
            {
                return BadRequest(new { Message = "Email already verified" });
            }

            verification.IsVerified = true;

            return Ok(new
            {
                Message = "Email verified successfully!",
                Email = request.Email,
                VerifiedAt = DateTime.UtcNow
            });
        }
    }
}