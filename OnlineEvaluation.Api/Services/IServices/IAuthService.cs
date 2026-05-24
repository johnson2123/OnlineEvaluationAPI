using Microsoft.AspNetCore.Identity;
using OnlineEvaluation.Api.Models.DTO;

namespace OnlineEvaluation.Api.Services.IServices
{
    public interface IAuthService
    {
        Task<RegisterResultDto> RegisterAsync(RegisterDto dto);
        Task<AuthResponseDto> LoginAsync(LoginDto dto);
        Task<AuthResponseDto> RefreshTokenAsync(string refreshToken);
        Task<bool> RevokeRefreshTokenAsync(string refreshToken);
        Task<bool> ConfirmEmailAsync(string userId, string token);
        Task<bool> ForgotPasswordAsync(string email);
        Task<bool> ResetPasswordAsync(ResetPasswordDto dto);
        Task<IdentityResult> ChangeInitialPasswordAsync(ChangeInitialPasswordDto dto);

        string ValidatePreAuthToken(string preAuthToken);
        Task<AuthResponseDto> GenerateFinalLoginTokensAsync(string userId);
        Task<string> GetUserMfaSecretKeyAsync(string userId);
    }
}
