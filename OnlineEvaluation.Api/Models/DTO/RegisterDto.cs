using System.ComponentModel.DataAnnotations;

namespace OnlineEvaluation.Api.Models.DTO
{
    public class RegisterDto
    {
        [Required]
        [EmailAddress]
        [MaxLength(256)]
        public string Email { get; set; }

        [Required]
        [MinLength(8)]
        [MaxLength(128)]
        public string Password { get; set; }

        [Required]
        [MaxLength(100)]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(100)]
        public string LastName { get; set; }

        // --- Role-based registration ---
        [Required]
        public string Role { get; set; }
    }
}
