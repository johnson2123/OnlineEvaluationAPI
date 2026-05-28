namespace OnlineEvaluation.Api.Models.Entities
{
    public class AccountLock
    {
        public long SecurityId { get; set; }
        public string UserId { get; set; } = string.Empty;
        public int FailedLoginAttempts { get; set; }
        public DateTime? LockedUntil { get; set; }
        public bool IsAccountLocked { get; set; }
        public DateTime? LastFailedLoginDate { get; set; }
        public DateTime? LastPasswordChangedDate { get; set; }
        public DateTime? PasswordExpiryDate { get; set; }
        public DateTime? LastSuccessfulLoginDate { get; set; }
        public bool SecurityQuestionEnabled { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }

        public virtual ApplicationUser User { get; set; } = null!;
    }
}
