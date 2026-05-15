using System.ComponentModel.DataAnnotations;

namespace OnlineEvaluation.Api.Models.DTO
{
    public class ChangeInitialPasswordDto
    {
        [Required]
        public string Email { get; set; }

        [Required]
        public string CurrentPassword { get; set; } // The generic admin-assigned password

        [Required]
        [MinLength(8)]
        public string NewPassword { get; set; }
    }
}
