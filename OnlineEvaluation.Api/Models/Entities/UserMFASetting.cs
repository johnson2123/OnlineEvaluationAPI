namespace OnlineEvaluation.Api.Models.Entities
{
    public class UserMFASetting
    {
        public int Id { get; set; }
        public string ApplicationUserId { get; set; } = string.Empty;
        public virtual ApplicationUser ApplicationUser { get; set; } = null!;

        public string MFAType { get; set; } = "None"; // "None", "AuthenticatorApp", "Email"
        public bool IsMFAEnabled { get; set; } = false;

        public string? SecretKey { get; set; }
        public string? BackupCodes { get; set; }
        public string? QRCodePath { get; set; } 
        public DateTime? LastUsedDate { get; set; } 

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }
}
