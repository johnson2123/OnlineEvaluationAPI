namespace OnlineEvaluation.Api.Models.Entities
{
    public class LoginAudit
    {
        public long AuditId { get; set; }
        public string UserId { get; set; } = string.Empty;
        public DateTime LoginTime { get; set; }
        public DateTime? LogoutTime { get; set; }
        public string? IPAddress { get; set; }
        public string? DeviceInfo { get; set; }
        public string? BrowserInfo { get; set; }
        public string? OperatingSystem { get; set; }
        public string LoginStatus { get; set; } = string.Empty; // Success, Failed, Locked, MFAFailed, SessionExpired, Logout
        public string? FailureReason { get; set; }
        public string? SessionId { get; set; }
        public string? LoginLocation { get; set; }
        public DateTime CreatedDate { get; set; }

        public virtual ApplicationUser User { get; set; } = null!;
    }
}
