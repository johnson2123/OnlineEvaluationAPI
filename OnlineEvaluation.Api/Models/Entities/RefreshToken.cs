using System.ComponentModel.DataAnnotations;

namespace OnlineEvaluation.Api.Models.Entities
{
    public class RefreshToken
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        [Required]
        [MaxLength(512)]
        public string? TokenHash { get; set; }
        public string? UserId { get; set; }
        public ApplicationUser User { get; set; }
        public DateTime ExpiresAt { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool Revoked { get; set; } = false;
        [MaxLength(512)]
        public string? ReplacedByTokenHash { get; set; }
        public bool IsActive => !Revoked && DateTime.UtcNow <= ExpiresAt;
    }
}
