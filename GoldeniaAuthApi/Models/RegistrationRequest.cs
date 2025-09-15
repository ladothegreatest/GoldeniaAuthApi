using System.ComponentModel.DataAnnotations;



namespace GoldeniaAuthApi.Models
{
    public class RegistrationRequest
    {
        [Required]
        [EmailAddress]
        public string Email {  get; set; } = string.Empty;

        [Required]
        [MinLength(2)]
        [MaxLength(255)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [MinLength(2)]
        [MaxLength(255)]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [Phone]
        public string PhoneNumber { get; set; } = string.Empty;
    }
}
