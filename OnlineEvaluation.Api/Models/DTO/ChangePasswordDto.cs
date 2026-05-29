using System.ComponentModel.DataAnnotations;

namespace OnlineEvaluation.Api.Models.DTO
{
    public class ChangePasswordDto
    {
        [Required]
        public string UserId { get; set; }

        [Required]
        public string CurrentPassword { get; set; }

        [Required]
        [MinLength(8)]
        public string NewPassword { get; set; }
    }
}
