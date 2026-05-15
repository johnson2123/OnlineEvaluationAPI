using System.ComponentModel.DataAnnotations;

namespace OnlineEvaluation.Api.Models.Entities
{
    public class RefreshToken
    {
        public Guid Id { get; set; }
        public string TokenHash { get; set; }
        public string UserId { get; set; }
        public virtual ApplicationUser User { get; set; }

        public DateTime ExpiresAt { get; set; }
        public DateTime CreatedAt { get; set; }

        public bool Revoked { get; set; }

        public string? ReplacedByTokenHash { get; set; }
        public bool IsActive => !Revoked && DateTime.UtcNow <= ExpiresAt;
    }
}
