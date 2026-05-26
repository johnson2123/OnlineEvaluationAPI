namespace OnlineEvaluation.Api.Models.Entities
{
    public class OtpLog
    {
        public int Id { get; set; }
        public Guid Guid { get; set; } = Guid.NewGuid();

        public string ApplicationUserId { get; set; } = string.Empty;
        public virtual ApplicationUser ApplicationUser { get; set; } = null!;

        public string OtpCode { get; set; } = null!;
        public string OtpType { get; set; } = null!;
        public string SentTo { get; set; } = null!;

        public DateTime ExpiryTime { get; set; }
        public bool IsUsed { get; set; } = false;
        public int AttemptCount { get; set; } = 0;

        public string? IPAddress { get; set; }
        public string? DeviceInfo { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    }
}
