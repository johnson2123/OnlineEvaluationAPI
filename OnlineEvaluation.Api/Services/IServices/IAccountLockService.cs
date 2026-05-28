namespace OnlineEvaluation.Api.Services.IServices
{
    public interface IAccountLockService
    {
        Task<bool> IsAccountLockedAsync(string userId);
        Task<bool> IncrementFailedAttemptsAsync(string userId, int maxAttempts = 5, int lockoutMinutes = 30);
        Task ResetFailedAttemptsAsync(string userId);
        Task InitializeAccountSecurityAsync(string userId);
        Task UpdatePasswordLifecycleAsync(string userId, int daysUntilExpiry = 90);
    }
}
