using System.ComponentModel.DataAnnotations;

namespace OnlineEvaluation.Api.Models.DTO
{
    public class PasswordChangeDto
    {
        [Required]
        public string CurrentPassword { get; set; }

        [Required]
        [MinLength(8)]
        public string NewPassword { get; set; }

        [Required]
        public string OtpCode { get; set; }
    }
}
