namespace OnlineEvaluation.Api.Services.IServices
{
    public interface IOtpService
    {
        Task<string> GenerateAndSaveOtpAsync(string userId, string email, string otpType, string? ipAddress = null, string? deviceInfo = null);
        Task<bool> VerifyOtpAsync(string userId, string inputOtp, string otpType);
    }
}
