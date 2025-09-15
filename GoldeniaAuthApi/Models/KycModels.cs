
using System.ComponentModel.DataAnnotations;

namespace GoldeniaAuthApi.Models
{
    public class KycDocument
    {
        public int Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public DocumentType DocumentType { get; set; }
        public string DocumentNumber { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public string FrontImagePath { get; set; } = string.Empty;
        public string? BackImagePath { get; set; }
        public KycStatus Status { get; set; } = KycStatus.Pending;
        public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;
        public string? RejectionReason { get; set; }
    }

    public class KycSubmissionRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public DocumentType DocumentType { get; set; }

        [Required]
        public string DocumentNumber { get; set; } = string.Empty;

        [Required]
        public string Country { get; set; } = string.Empty;
    }

    public enum DocumentType
    {
        Passport = 1,
        DriverLicense = 2,
        NationalId = 3
    }

    public enum KycStatus
    {
        Pending = 1,
        Approved = 2,
        Rejected = 3
    }
}