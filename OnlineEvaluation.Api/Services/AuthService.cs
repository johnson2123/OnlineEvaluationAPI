
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OnlineEvaluation.Api.Data;
using OnlineEvaluation.Api.Models;
using OnlineEvaluation.Api.Models.DTO;
using OnlineEvaluation.Api.Models.Entities;
using OnlineEvaluation.Api.Services.Helpers;
using OnlineEvaluation.Api.Services.IServices;

namespace OnlineEvaluation.Api.Services
{
    public class AuthService :IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ITokenService _tokenService;
        private readonly IMfaSecurityService _mfaSecurity;
        private readonly ApplicationDbContext _db;
        private readonly IEmailService _emailService;
        private readonly IConfiguration _config;
        private readonly string _refeshTokenHashKey;

        public AuthService(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            ITokenService tokenService,
            ApplicationDbContext db,
            IEmailService emailService,
            IConfiguration config,
            IMfaSecurityService mfaSecurity)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _tokenService = tokenService;
            _db = db;
            _emailService = emailService;
            _config = config;
            _mfaSecurity = mfaSecurity;
            _refeshTokenHashKey = _config["Jwt:RefreshTokenHashKey"];
        }

        public async Task<RegisterResultDto> RegisterAsync(RegisterDto dto)
        {
            var existing = await _userManager.FindByEmailAsync(dto.Email);
            if (existing != null)
            {
                return new RegisterResultDto(false, new[] { "Email already in use" });
            }

            using var transaction = await _db.Database.BeginTransactionAsync();
            try
            {
                var user = new ApplicationUser
                {
                    UserName = dto.Email,
                    Email = dto.Email,
                    FirstName = dto.FirstName,
                    LastName = dto.LastName,
                    IsActive = true,
                    MustChangePassword = true,
                    EmailConfirmed = true
                };

                var result = await _userManager.CreateAsync(user, dto.Password);
                if (!result.Succeeded)
                {
                    return new RegisterResultDto(false, result.Errors.Select(u => u.Description));
                }

                var mfaSetting = new UserMFASetting
                {
                    ApplicationUserId = user.Id,
                    IsMFAEnabled = dto.IsMfaEnabled,
                    MFAType = dto.MFAType,
                    CreatedAt = DateTime.UtcNow
                };

                if (dto.IsMfaEnabled && dto.MFAType == "AuthenticatorApp")
                {
                    mfaSetting.SecretKey = string.IsNullOrWhiteSpace(dto.SecretKey)
                        ? _mfaSecurity.GenerateRandomSecretKey()
                        : dto.SecretKey;

                    var backupCodesList = _mfaSecurity.GenerateBackupCodes();
                    mfaSetting.BackupCodes = string.Join(",", backupCodesList);
                }

                await _db.UserMFASettings.AddAsync(mfaSetting);
                await _db.SaveChangesAsync();

                var assignedRole = string.IsNullOrWhiteSpace(dto.Role) ? "User" : dto.Role;

                if (!await _roleManager.RoleExistsAsync(assignedRole))
                {
                    await _roleManager.CreateAsync(new IdentityRole(assignedRole));
                }

                await _userManager.AddToRoleAsync(user, assignedRole);

                await transaction.CommitAsync();

                return new RegisterResultDto(true, Array.Empty<string>());
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return new RegisterResultDto(false, new[] { $"An error occurred during registration: {ex.Message}" });
            }
        
        }

        public string ValidatePreAuthToken(string preAuthToken)
        {
            if (string.IsNullOrWhiteSpace(preAuthToken)) return string.Empty;

            try
            {
                return _tokenService.ValidateTokenAndGetUserId(preAuthToken);
            }
            catch(Exception ex)
            {
                Console.WriteLine($"[PRE-AUTH WRAPPER CRASH]: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"[PRE-AUTH INNER]: {ex.InnerException.Message}");
                }
                return string.Empty;
            }
        }

        public async Task<string> GetUserMfaSecretKeyAsync(string userId)
        {
            if (string.IsNullOrEmpty(userId)) return string.Empty;

            var mfaSetting = await _db.UserMFASettings
                .FirstOrDefaultAsync(m => m.ApplicationUserId == userId);

            return mfaSetting?.SecretKey ?? string.Empty;
        }

        public async Task<AuthResponseDto> LoginAsync(LoginDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);

            if (user == null) throw new UnauthorizedAccessException("Invalid credentials");

            if (!user.IsActive) throw new UnauthorizedAccessException("User is inactive");

            if (!await _userManager.CheckPasswordAsync(user, dto.Password))
                throw new UnauthorizedAccessException("Invalid credentials");

            if (_userManager.Options.SignIn.RequireConfirmedEmail && !user.EmailConfirmed)
                throw new UnauthorizedAccessException("Email not confirmed");

            if (user.MustChangePassword)
            {
                var rolecheck = await _userManager.GetRolesAsync(user);

                if (rolecheck.Contains("Admin"))
                {
                    user.MustChangePassword = false;
                    await _userManager.UpdateAsync(user);
                }
                else
                {

                    return new AuthResponseDto
                    {
                        RequiresPasswordChange = true
                    };
                }
            }

            var mfaSetting = await _db.Set<UserMFASetting>()
                .FirstOrDefaultAsync(m => m.ApplicationUserId == user.Id);

            if (mfaSetting != null)
            {
                // CASE A: Standard Daily Login (MFA is fully verified and enabled)
                if (mfaSetting.IsMFAEnabled)
                {
                    var preAuthToken = _tokenService.GeneratePreAuthToken(user);

                    return new AuthResponseDto
                    {
                        RequiresPasswordChange = false,
                        IsMfaRequired = true,
                        RequiresMfaSetup = false,
                        PreAuthToken = preAuthToken.Token,
                        MfaType = mfaSetting.MFAType
                    };
                }

                if (!mfaSetting.IsMFAEnabled && !string.IsNullOrEmpty(mfaSetting.SecretKey))
                {
                    var freshPreAuthToken = _tokenService.GeneratePreAuthToken(user);

                    string keyUri = _mfaSecurity.GenerateQrCodeUri(user.Email!, mfaSetting.SecretKey);
                    string base64Image = "";

                    // Re-render the visual QR code byte stream from the existing key
                    using (var qrGenerator = new QRCoder.QRCodeGenerator())
                    using (var qrCodeData = qrGenerator.CreateQrCode(keyUri, QRCoder.QRCodeGenerator.ECCLevel.Q))
                    using (var qrCode = new QRCoder.PngByteQRCode(qrCodeData))
                    {
                        byte[] qrCodeBytes = qrCode.GetGraphic(20);
                        base64Image = $"data:image/png;base64,{Convert.ToBase64String(qrCodeBytes)}";
                    }

                    return new AuthResponseDto
                    {
                        RequiresPasswordChange = false,
                        IsMfaRequired = false,
                        RequiresMfaSetup = true,
                        PreAuthToken = freshPreAuthToken.Token,
                        QrCodeBase64 = base64Image,
                        SharedSecret = mfaSetting.SecretKey,
                        MfaType = mfaSetting.MFAType
                    };
                }

            }

                return await GenerateFinalLoginTokensAsync(user.Id);
        }

        public async Task<AuthResponseDto> GenerateFinalLoginTokensAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null || !user.IsActive) throw new UnauthorizedAccessException("Session validation failed.");

            var roles = await _userManager.GetRolesAsync(user);
            var tokenResult = _tokenService.GenerateAccessToken(user, roles);

            var refreshToken = _tokenService.GenerateRefreshToken();
            var refreshTokenHash = TokenHelpers.ComputeHmacSha256Base64(refreshToken, _refeshTokenHashKey);
            var expiry = DateTime.UtcNow.AddMinutes(Convert.ToDouble(_config["Jwt:RefreshTokenExpireMinutes"] ?? "60"));

            await using var tx = await _db.Database.BeginTransactionAsync();
            try
            {
                var activeTokens = await _db.RefreshTokens
                    .Where(t => t.UserId == user.Id && !t.Revoked && t.ExpiresAt > DateTime.UtcNow)
                    .ToListAsync();

                foreach (var t in activeTokens)
                {
                    t.Revoked = true;
                    t.ReplacedByTokenHash = refreshTokenHash;
                }

                var refreshTokenEntity = new RefreshToken
                {
                    TokenHash = refreshTokenHash,
                    UserId = user.Id,
                    ExpiresAt = expiry,
                    CreatedAt = DateTime.UtcNow
                };

                _db.RefreshTokens.Add(refreshTokenEntity);
                await _db.SaveChangesAsync();
                await tx.CommitAsync();
            }
            catch
            {
                await tx.RollbackAsync();
                throw;
            }

            return new AuthResponseDto
            {
                AccessToken = tokenResult.Token,
                RefreshToken = refreshToken,
                AccessTokenExpiresAt = tokenResult.ExpiresAtUtc,
                RequiresPasswordChange = false,
                IsMfaRequired = false
            };
        }

        public async Task<AuthResponseDto> RefreshTokenAsync(string refreshToken)
        {
            if (string.IsNullOrWhiteSpace(refreshToken))
                throw new UnauthorizedAccessException("Refresh token is required.");

            var incomingHash = TokenHelpers.ComputeHmacSha256Base64(refreshToken, _refeshTokenHashKey);

            var tokenEntity = await _db.RefreshTokens
                                        .FirstOrDefaultAsync(u => u.TokenHash == incomingHash);

            if (tokenEntity == null)
                throw new UnauthorizedAccessException("Invalid refresh token");

            if (tokenEntity.Revoked || tokenEntity.ExpiresAt <= DateTime.UtcNow)
            {
                // Revoke all tokens for this user as a security measure
                var allTokens = await _db.RefreshTokens
                    .Where(t => t.UserId == tokenEntity.UserId && !t.Revoked)
                    .ToListAsync();

                foreach (var t in allTokens) t.Revoked = true;
                await _db.SaveChangesAsync();

                throw new UnauthorizedAccessException("Invalid refresh token");
            }

            var user = await _userManager.FindByIdAsync(tokenEntity.UserId);
            if (user == null || !user.IsActive)
                throw new UnauthorizedAccessException("User no longer exists or is inactive.");

            // Update the old token and create the new one (Token Rotation)
            tokenEntity.Revoked = true;

            var newRefreshToken = _tokenService.GenerateRefreshToken();
            var newRefreshTokenHash = TokenHelpers.ComputeHmacSha256Base64(newRefreshToken, _refeshTokenHashKey);
            tokenEntity.ReplacedByTokenHash = newRefreshTokenHash;

            var newTokenEntity = new RefreshToken
            {
                TokenHash = newRefreshTokenHash,
                UserId = user.Id,
                ExpiresAt = DateTime.UtcNow.AddMinutes(Convert.ToDouble(_config["Jwt:RefreshTokenExpireMinutes"] ?? "60")),
                CreatedAt = DateTime.UtcNow
            };

            _db.RefreshTokens.Add(newTokenEntity);

            await _db.SaveChangesAsync();

            var roles = await _userManager.GetRolesAsync(user);
            var accessTokenResult = _tokenService.GenerateAccessToken(user, roles);

            return new AuthResponseDto
            {
                AccessToken = accessTokenResult.Token,
                RefreshToken = newRefreshToken,
                AccessTokenExpiresAt = accessTokenResult.ExpiresAtUtc
            };

        }

        public async Task<bool> RevokeRefreshTokenAsync(string refreshToken)
        {
            var incomingHash = TokenHelpers.ComputeHmacSha256Base64(refreshToken, _refeshTokenHashKey);
            var tokenEntity = await _db.RefreshTokens.FirstOrDefaultAsync(u => u.TokenHash == incomingHash);
            if (tokenEntity == null || tokenEntity.Revoked)
            {
                return false;
            }
            tokenEntity.Revoked = true;
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<SetupMfaResponse> ChangeInitialPasswordAsync(ChangeInitialPasswordDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null)
                return new SetupMfaResponse { Succeeded = false, Errors = new[] { "User context not found." } };

            if (!user.MustChangePassword)
            {
                return new SetupMfaResponse { Succeeded = false, Errors = new[] { "Initial setup already completed." } };
            }

            var result = await _userManager.ChangePasswordAsync(user, dto.CurrentPassword, dto.NewPassword);

            if (!result.Succeeded)
            {
                return new SetupMfaResponse
                {
                    Succeeded = false,
                    Errors = result.Errors.Select(e => e.Description)
                };
            }

            user.MustChangePassword = false;
            user.IsActive = true;
            await _userManager.UpdateAsync(user);

            var mfaSetting = await _db.UserMFASettings
                                    .FirstOrDefaultAsync(m => m.ApplicationUserId == user.Id);

            var roles = await _userManager.GetRolesAsync(user);
            bool isMfaMandatoryRole = roles.Contains("Admin") ||
                                      roles.Contains("Controller") ||
                                      roles.Contains("Faculty") ||
                                      roles.Contains("Moderator");

            if (isMfaMandatoryRole)
            {
                string randomSecret = _mfaSecurity.GenerateRandomSecretKey();
                var backupCodesList = _mfaSecurity.GenerateBackupCodes();

                if (mfaSetting == null)
                {
                    mfaSetting = new UserMFASetting
                    {
                        ApplicationUserId = user.Id,
                        CreatedAt = DateTime.UtcNow
                    };
                    await _db.UserMFASettings.AddAsync(mfaSetting);
                }

                mfaSetting.MFAType = "AuthenticatorApp";
                mfaSetting.IsMFAEnabled = false; // Remains false until they complete setup code validation
                mfaSetting.SecretKey = randomSecret;
                mfaSetting.BackupCodes = string.Join(",", backupCodesList);
                mfaSetting.UpdatedAt = DateTime.UtcNow;

                _db.UserMFASettings.Update(mfaSetting);
                await _db.SaveChangesAsync();

                string keyUri = _mfaSecurity.GenerateQrCodeUri(user.Email!, randomSecret);

                using (var qrGenerator = new QRCoder.QRCodeGenerator())
                using (var qrCodeData = qrGenerator.CreateQrCode(keyUri, QRCoder.QRCodeGenerator.ECCLevel.Q))
                using (var qrCode = new QRCoder.PngByteQRCode(qrCodeData))
                {
                    byte[] qrCodeBytes = qrCode.GetGraphic(20);
                    string base64Image = $"data:image/png;base64,{Convert.ToBase64String(qrCodeBytes)}";

                    var preAuthTokenResult = _tokenService.GeneratePreAuthToken(user);

                    return new SetupMfaResponse
                    {
                        Succeeded = true,
                        RequiresMfaSetup = true,
                        QrCodeBase64 = base64Image,
                        SharedSecret = randomSecret, // Raw key for user string fallbacks
                        PreAuthToken = preAuthTokenResult.Token
                    };
                }
            }

            return new SetupMfaResponse { Succeeded = true, RequiresMfaSetup = false };
        }

        public async Task ActivateMfaAsync(string userId)
        {
            var mfaSetting = await _db.UserMFASettings
                                    .FirstOrDefaultAsync(m => m.ApplicationUserId == userId);

            if (mfaSetting != null && !mfaSetting.IsMFAEnabled)
            {
                mfaSetting.IsMFAEnabled = true;
                mfaSetting.UpdatedAt = DateTime.UtcNow;

                _db.UserMFASettings.Update(mfaSetting);
                await _db.SaveChangesAsync();
            }
        }

        public Task<bool> ConfirmEmailAsync(string userId, string token)
        {
            // Implementation Pending
            return Task.FromResult(false);
        }

        public Task<bool> ForgotPasswordAsync(string email)
        {
            // Implementation Pending
            return Task.FromResult(false);
        }


        public Task<bool> ResetPasswordAsync(ResetPasswordDto dto)
        {
            // Implementation Pending
            return Task.FromResult(false);
        }

        
    }
}
