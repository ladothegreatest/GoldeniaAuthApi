using Microsoft.AspNetCore.Mvc;
using GoldeniaAuthApi.Models;
using GoldeniaAuthApi.Services;
using GoldeniaAuthApi.Data;
using System.ComponentModel.DataAnnotations;

namespace GoldeniaKycApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SmsController : ControllerBase
    {
        private readonly ISmsService _smsService;

        public SmsController(ISmsService smsService)
        {
            _smsService = smsService;
        }

        [HttpPost("send-verification")]
        public async Task<IActionResult> SendSmsVerification([FromBody] SendSmsRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // this generates SMS verification code
            var verificationCode = new Random().Next(100000, 999999).ToString();

            // this stores SMS verification in datastore
            DataStore.SmsVerifications.Add(new SmsVerification
            {
                Id = new Random().Next(1, 1000),
                PhoneNumber = request.PhoneNumber,
                VerificationCode = verificationCode,
                IsVerified = false,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddMinutes(15)
            });

            try
            {
                // sends SMS
                await _smsService.SendSmsCodeAsync(request.PhoneNumber, verificationCode);

                return Ok(new
                {
                    Message = "SMS verification code sent successfully",
                    PhoneNumber = request.PhoneNumber
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Failed to send SMS verification code" });
            }
        }

        [HttpPost("verify")]
        public IActionResult VerifySms([FromBody] VerifySmsRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var verification = DataStore.SmsVerifications
                .FirstOrDefault(v => v.PhoneNumber == request.PhoneNumber && v.VerificationCode == request.Code);

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
                return BadRequest(new { Message = "Phone number already verified" });
            }

            verification.IsVerified = true;

            return Ok(new
            {
                Message = "Phone number verified successfully!",
                PhoneNumber = request.PhoneNumber,
                VerifiedAt = DateTime.UtcNow
            });
        }
    }

    public class SendSmsRequest
    {
        [Required]
        [Phone]
        public string PhoneNumber { get; set; } = string.Empty;
    }
}
