namespace OnlineEvaluation.Api.Services.IServices
{
    public interface ILoginAuditService
    {
        Task LogLoginAttemptAsync(
            string userId,
            string status,
            string? failureReason = null,
            string? ipAddress = null,
            string? browserInfo = null,
            string? osInfo = null,
            string? deviceInfo = null,
            string? sessionId = null);
        Task LogLogoutAsync(string userId, string sessionId);
    }
}
