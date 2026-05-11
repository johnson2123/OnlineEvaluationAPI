using System.ComponentModel.DataAnnotations;

namespace OnlineEvaluation.Api.Models.DTO
{
    public class RefreshRequestDto
    {
        [Required]
        [MinLength(16)]
        public string? RefreshToken { get; set; }
    }
}
