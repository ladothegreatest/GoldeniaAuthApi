
using Microsoft.AspNetCore.Mvc;
using GoldeniaAuthApi.Models;
using GoldeniaAuthApi.Data;

namespace GoldeniaAuthApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class KycController : ControllerBase
    {
        [HttpPost("submit")]
        public IActionResult SubmitKyc([FromBody] KycSubmissionRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Checks if user has verified email
            var emailVerification = DataStore.EmailVerifications
                .FirstOrDefault(v => v.Email == request.Email && v.IsVerified);

            if (emailVerification == null)
            {
                return BadRequest(new { Message = "Please verify your email before submitting KYC documents" });
            }

            // Checks if KYC already submitted
            var existingKyc = DataStore.KycDocuments.FirstOrDefault(k => k.Email == request.Email);
            if (existingKyc != null)
            {
                return BadRequest(new { Message = "KYC documents already submitted for this email" });
            }


            // Creates KYC document
            var kycDocument = new KycDocument
            {
                Id = new Random().Next(1000, 9999),
                Email = request.Email,
                DocumentType = request.DocumentType,
                DocumentNumber = request.DocumentNumber,
                Country = request.Country,
                FrontImagePath = $"documents/{request.Email}_front.jpg",
                Status = KycStatus.Pending,
                SubmittedAt = DateTime.UtcNow
            };

            DataStore.KycDocuments.Add(kycDocument);

            return Ok(new
            {
                Message = "KYC documents submitted successfully",
                KycId = kycDocument.Id,
                Status = kycDocument.Status.ToString(),
                SubmittedAt = kycDocument.SubmittedAt
            });
        }

        [HttpGet("status/{email}")]
        public IActionResult GetKycStatus(string email)
        {
            var kycDocument = DataStore.KycDocuments.FirstOrDefault(k => k.Email == email);

            if (kycDocument == null)
            {
                return NotFound(new { Message = "No KYC documents found for this email" });
            }

            return Ok(new
            {
                KycId = kycDocument.Id,
                Email = kycDocument.Email,
                DocumentType = kycDocument.DocumentType.ToString(),
                Status = kycDocument.Status.ToString(),
                SubmittedAt = kycDocument.SubmittedAt,
                RejectionReason = kycDocument.RejectionReason
            });
        }

        [HttpPost("approve/{kycId}")]
        public IActionResult ApproveKyc(int kycId)
        {
            var kycDocument = DataStore.KycDocuments.FirstOrDefault(k => k.Id == kycId);

            if (kycDocument == null)
            {
                return NotFound(new { Message = "KYC document not found" });
            }

            kycDocument.Status = KycStatus.Approved;

            return Ok(new
            {
                Message = "KYC approved successfully",
                KycId = kycDocument.Id,
                Status = kycDocument.Status.ToString()
            });
        }
    }
}
