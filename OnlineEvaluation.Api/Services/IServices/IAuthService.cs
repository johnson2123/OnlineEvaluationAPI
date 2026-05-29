using Microsoft.AspNetCore.Identity;
using OnlineEvaluation.Api.Models.DTO;

namespace OnlineEvaluation.Api.Services.IServices
{
    public interface IAuthService
    {
        Task<RegisterResultDto> RegisterAsync(RegisterDto dto);
        Task<AuthResponseDto> LoginAsync(LoginDto dto, ClientMetadataDto? clientMetadata = null);
        Task<AuthResponseDto> RefreshTokenAsync(string refreshToken);
        Task<bool> RevokeRefreshTokenAsync(string refreshToken);
        Task<bool> ConfirmEmailAsync(string userId, string token);
        Task<bool> ForgotPasswordAsync(string email);
        Task<bool> ResetPasswordAsync(ResetPasswordDto dto);
        Task<bool> SendPasswordChangeOtpAsync(string userId);
        Task<IdentityResult> PasswordChangeAsync(string userId, PasswordChangeDto dto);
        Task<SetupMfaResponse> ChangeInitialPasswordAsync(ChangeInitialPasswordDto dto);
        Task<SetupMfaResponse> VerifyMfaOnboardingOtpAsync(VerifyMfaOnboardingOtpDto dto);
        Task<bool> VerifyAndTrackMfaAppCodeAsync(string userId, string code);
        Task<bool> VerifyAndTrackEmailOtpAsync(string userId, string code);
        Task ActivateMfaAsync(string userId);
        string ValidatePreAuthToken(string preAuthToken);
        Task<AuthResponseDto> GenerateFinalLoginTokensAsync(string userId, ClientMetadataDto? clientMetadata = null);
        Task<string> GetUserMfaSecretKeyAsync(string userId);
    }
}
