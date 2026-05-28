
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OnlineEvaluation.Api.Data;
using OnlineEvaluation.Api.Models;
using OnlineEvaluation.Api.Models.DTO;
using OnlineEvaluation.Api.Models.Entities;
using OnlineEvaluation.Api.Services.Background;
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
        private readonly IOtpService _otpService;
        private readonly ApplicationDbContext _db;
        private readonly IEmailQueue _emailQueue;
        private readonly IConfiguration _config;
        private readonly string _refreshTokenHashKey;
        private readonly IAccountLockService _accountLockService;
        private readonly ILoginAuditService _loginAuditService;

        public AuthService(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            ITokenService tokenService,
            ApplicationDbContext db,
            IEmailQueue emailQueue,
            IConfiguration config,
            IMfaSecurityService mfaSecurity,
            IOtpService otpService,
            IAccountLockService accountLockService, 
            ILoginAuditService loginAuditService)

        {
            _userManager = userManager;
            _roleManager = roleManager;
            _tokenService = tokenService;
            _db = db;
            _emailQueue = emailQueue;
            _config = config;
            _mfaSecurity = mfaSecurity;
            _otpService = otpService;
            _accountLockService = accountLockService;
            _loginAuditService = loginAuditService;
            _refreshTokenHashKey = _config["Jwt:RefreshTokenHashKey"];
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

                await _accountLockService.InitializeAccountSecurityAsync(user.Id);

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

        public async Task<AuthResponseDto> LoginAsync(LoginDto dto, ClientMetadataDto? clientMetadata = null)
        {
            string ip = clientMetadata?.IPAddress ?? "Unknown";
            string browser = clientMetadata?.BrowserInfo ?? "Unknown";
            string os = clientMetadata?.OperatingSystem ?? "Unknown";
            string device = clientMetadata?.DeviceInfo ?? "Unknown";

            var user = await _userManager.FindByEmailAsync(dto.Email);

            if (user == null) throw new UnauthorizedAccessException("Invalid credentials");

            if (!user.IsActive) throw new UnauthorizedAccessException("User is inactive");

            bool isLocked = await _accountLockService.IsAccountLockedAsync(user.Id);
            if (isLocked)
            {
                await _loginAuditService.LogLoginAttemptAsync(
                    userId: user.Id,
                    status: "Locked",
                    failureReason: "Account is temporarily locked out due to multiple failed attempts.",
                    ipAddress: ip,
                    browserInfo: browser,
                    osInfo: os,
                    deviceInfo: device
                );

                throw new UnauthorizedAccessException("This account is temporarily locked. Please try again later.");
            }

            if (!await _userManager.CheckPasswordAsync(user, dto.Password))
            {
                bool didTriggerLockout = await _accountLockService.IncrementFailedAttemptsAsync(user.Id, maxAttempts: 5, lockoutMinutes: 30);
                string status = didTriggerLockout ? "Locked" : "Failed";
                string reason = didTriggerLockout
                    ? "Account threshold breached. Account locked for 30 minutes."
                    : "Invalid password credential provided.";

                await _loginAuditService.LogLoginAttemptAsync(
                                        userId: user.Id,
                                        status: status,
                                        failureReason: reason,
                                        ipAddress: ip,
                                        browserInfo: browser,
                                        osInfo: os,
                                        deviceInfo: device
                                    );

                throw new UnauthorizedAccessException("Invalid credentials");
            }

            await _accountLockService.ResetFailedAttemptsAsync(user.Id);

            if (_userManager.Options.SignIn.RequireConfirmedEmail && !user.EmailConfirmed)
            {
                await _loginAuditService.LogLoginAttemptAsync(
                    userId: user.Id, status: "Failed", failureReason: "Email verification is incomplete.",
                    ipAddress: ip, browserInfo: browser, osInfo: os, deviceInfo: device
                );

                throw new UnauthorizedAccessException("Email not confirmed");
            }
                

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
                    await _loginAuditService.LogLoginAttemptAsync(
                        userId: user.Id, status: "Partial", failureReason: "Authentication paused. Initial password change mandated.",
                        ipAddress: ip, browserInfo: browser, osInfo: os, deviceInfo: device
                    );

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
                if (mfaSetting.IsMFAEnabled)
                {
                    var preAuthToken = _tokenService.GeneratePreAuthToken(user);

                    if (mfaSetting.MFAType == "Email")
                    {
                        string generatedOtp = await _otpService.GenerateAndSaveOtpAsync(
                            user.Id,
                            user.Email!,
                            "Login",
                            null,
                            null
                        );

                        await SendEmailChallengeAsync(user.Email!, user.FirstName ?? "User", generatedOtp);
                    }

                    await _loginAuditService.LogLoginAttemptAsync(
                        userId: user.Id, status: "MfaChallenge", failureReason: $"MFA verification challenged via {mfaSetting.MFAType}.",
                        ipAddress: ip, browserInfo: browser, osInfo: os, deviceInfo: device
                    );

                    return new AuthResponseDto
                    {
                        RequiresPasswordChange = false,
                        IsMfaRequired = true,
                        RequiresMfaSetup = false,
                        PreAuthToken = preAuthToken.Token,
                        MfaType = mfaSetting.MFAType
                    };
                }

                if (!mfaSetting.IsMFAEnabled)
                {
                    if (mfaSetting.MFAType == "AuthenticatorApp" && !string.IsNullOrEmpty(mfaSetting.SecretKey))
                    {
                        var freshPreAuthToken = _tokenService.GeneratePreAuthToken(user);
                        string keyUri = _mfaSecurity.GenerateQrCodeUri(user.Email!, mfaSetting.SecretKey);
                        string base64Image = "";

                        using (var qrGenerator = new QRCoder.QRCodeGenerator())
                        using (var qrCodeData = qrGenerator.CreateQrCode(keyUri, QRCoder.QRCodeGenerator.ECCLevel.Q))
                        using (var qrCode = new QRCoder.PngByteQRCode(qrCodeData))
                        {
                            byte[] qrCodeBytes = qrCode.GetGraphic(20);
                            base64Image = $"data:image/png;base64,{Convert.ToBase64String(qrCodeBytes)}";
                        }

                        await _loginAuditService.LogLoginAttemptAsync(
                            userId: user.Id, status: "Partial", failureReason: "MFA onboarding configuration setup requested.",
                            ipAddress: ip, browserInfo: browser, osInfo: os, deviceInfo: device
                        );

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
                    else if (mfaSetting.MFAType == "AuthenticatorApp")
                    {
                        // Fix: Ensure secret key always exists if onboarding is incomplete
                        if (string.IsNullOrEmpty(mfaSetting.SecretKey))
                        {
                            mfaSetting.SecretKey = _mfaSecurity.GenerateRandomSecretKey();
                            var backupCodesList = _mfaSecurity.GenerateBackupCodes();
                            mfaSetting.BackupCodes = string.Join(",", backupCodesList);
                            await _db.SaveChangesAsync();
                        }

                        var freshPreAuthToken = _tokenService.GeneratePreAuthToken(user);
                        string keyUri = _mfaSecurity.GenerateQrCodeUri(user.Email!, mfaSetting.SecretKey);
                        string base64Image = "";

                        using (var qrGenerator = new QRCoder.QRCodeGenerator())
                        using (var qrCodeData = qrGenerator.CreateQrCode(keyUri, QRCoder.QRCodeGenerator.ECCLevel.Q))
                        using (var qrCode = new QRCoder.PngByteQRCode(qrCodeData))
                        {
                            byte[] qrCodeBytes = qrCode.GetGraphic(20);
                            base64Image = $"data:image/png;base64,{Convert.ToBase64String(qrCodeBytes)}";
                        }

                        await _loginAuditService.LogLoginAttemptAsync(
                            userId: user.Id, status: "Partial", failureReason: "MFA onboarding initialization requested.",
                            ipAddress: ip, browserInfo: browser, osInfo: os, deviceInfo: device
                        );

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
            }

            return await GenerateFinalLoginTokensAsync(user.Id, clientMetadata);
        }

        public async Task<AuthResponseDto> GenerateFinalLoginTokensAsync(string userId, ClientMetadataDto? clientMetadata = null)
        {
            string ip = clientMetadata?.IPAddress ?? "Unknown";
            string browser = clientMetadata?.BrowserInfo ?? "Unknown";
            string os = clientMetadata?.OperatingSystem ?? "Unknown";
            string device = clientMetadata?.DeviceInfo ?? "Unknown";

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null || !user.IsActive) throw new UnauthorizedAccessException("Session validation failed.");

            var roles = await _userManager.GetRolesAsync(user);
            var tokenResult = _tokenService.GenerateAccessToken(user, roles);

            var handler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(tokenResult.Token);
            string jtiSessionId = jwtToken.Id;

            var refreshToken = _tokenService.GenerateRefreshToken();
            var refreshTokenHash = TokenHelpers.ComputeHmacSha256Base64(refreshToken, _refreshTokenHashKey);
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

                var successAuditEntry = new LoginAudit
                {
                    UserId = user.Id,
                    LoginStatus = "Success",
                    IPAddress = ip,
                    BrowserInfo = browser,
                    OperatingSystem = os,
                    DeviceInfo = device,
                    SessionId = jtiSessionId, 
                    LoginTime = DateTime.UtcNow,
                    CreatedDate = DateTime.UtcNow
                };
                await _db.LoginAudits.AddAsync(successAuditEntry);

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

            var incomingHash = TokenHelpers.ComputeHmacSha256Base64(refreshToken, _refreshTokenHashKey);

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
            var newRefreshTokenHash = TokenHelpers.ComputeHmacSha256Base64(newRefreshToken, _refreshTokenHashKey);
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
            var incomingHash = TokenHelpers.ComputeHmacSha256Base64(refreshToken, _refreshTokenHashKey);
            var tokenEntity = await _db.RefreshTokens.FirstOrDefaultAsync(u => u.TokenHash == incomingHash);
            if (tokenEntity == null || tokenEntity.Revoked)
            {
                return false;
            }
            tokenEntity.Revoked = true;

            var logoutAuditEntry = new LoginAudit
            {
                UserId = tokenEntity.UserId,
                LoginStatus = "Logout",
                FailureReason = "User explicitly logged out and terminated token context.",
                IPAddress = "N/A", 
                BrowserInfo = "N/A",
                OperatingSystem = "N/A",
                DeviceInfo = "N/A",
                LoginTime = DateTime.UtcNow,
                CreatedDate = DateTime.UtcNow
            };
            await _db.LoginAudits.AddAsync(logoutAuditEntry);

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

            using var transaction = await _db.Database.BeginTransactionAsync();
            try
            {
                var result = await _userManager.ChangePasswordAsync(user, dto.CurrentPassword, dto.NewPassword);
                if (!result.Succeeded)
                {
                    return new SetupMfaResponse { Succeeded = false, Errors = result.Errors.Select(e => e.Description) };
                }

                user.MustChangePassword = false;
                user.IsActive = true;
                var updateResult = await _userManager.UpdateAsync(user);

                if (!updateResult.Succeeded)
                {
                    return new SetupMfaResponse { Succeeded = false, Errors = updateResult.Errors.Select(e => e.Description) };
                }

                var mfaSetting = await _db.UserMFASettings
                    .FirstOrDefaultAsync(m => m.ApplicationUserId == user.Id);

                if (mfaSetting == null)
                {
                    mfaSetting = new UserMFASetting
                    {
                        ApplicationUserId = user.Id,
                        IsMFAEnabled = false,
                        MFAType = "None",
                        CreatedAt = DateTime.UtcNow
                    };
                    await _db.UserMFASettings.AddAsync(mfaSetting);
                    await _db.SaveChangesAsync();
                }

                bool structuralMfaSetupRequired = false;
                string? preAuthToken = null;

                if (mfaSetting.MFAType == "Email")
                {
                    mfaSetting.IsMFAEnabled = true;
                    mfaSetting.UpdatedAt = DateTime.UtcNow;
                    await _db.SaveChangesAsync();
                }
                else if (mfaSetting.MFAType == "AuthenticatorApp")
                {
                    string verificationOtp = await _otpService.GenerateAndSaveOtpAsync(
                        user.Id,
                        user.Email!,
                        "MfaOnboardingVerification",
                        null,
                        null
                    );

                    var preAuthTokenResult = _tokenService.GeneratePreAuthToken(user);
                    preAuthToken = preAuthTokenResult.Token;
                    await SendEmailChallengeAsync(user.Email!, user.FirstName ?? "User", verificationOtp);
                    structuralMfaSetupRequired = true;
                }

                await transaction.CommitAsync();

                return new SetupMfaResponse
                {
                    Succeeded = true,
                    RequiresMfaSetup = structuralMfaSetupRequired,
                    PreAuthToken = preAuthToken
                };
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return new SetupMfaResponse
                {
                    Succeeded = false,
                    Errors = new[] { $"An unexpected error occurred during password change setup: {ex.Message}" }
                };
            }

        }

        public async Task<SetupMfaResponse> VerifyMfaOnboardingOtpAsync(VerifyMfaOnboardingOtpDto dto)
        {
            string validatedUserId = ValidatePreAuthToken(dto.PreAuthToken);
            if (string.IsNullOrEmpty(validatedUserId))
            {
                return new SetupMfaResponse { Succeeded = false, Errors = new[] { "Session expired or invalid token." } };
            }

            if (await _accountLockService.IsAccountLockedAsync(validatedUserId))
            {
                return new SetupMfaResponse { Succeeded = false, Errors = new[] { "Account is locked out due to excessive failed verification attempts." } };
            }

            var user = await _userManager.FindByIdAsync(validatedUserId);
            if (user == null || !user.IsActive || !string.Equals(user.Email, dto.Email, StringComparison.OrdinalIgnoreCase))
            {
                return new SetupMfaResponse { Succeeded = false, Errors = new[] { "User context mismatch or account is disabled." } };
            }

            bool isOtpValid = await _otpService.VerifyOtpAsync(user.Id, dto.OtpCode, "MfaOnboardingVerification"); 
            if (!isOtpValid)
            {
                await _accountLockService.IncrementFailedAttemptsAsync(user.Id, 5, 30);
                return new SetupMfaResponse { Succeeded = false, Errors = new[] { "Invalid or expired verification code." } };
            }

            await _accountLockService.ResetFailedAttemptsAsync(user.Id);

            var mfaSetting = await _db.UserMFASettings
                .FirstOrDefaultAsync(m => m.ApplicationUserId == user.Id);

            if (mfaSetting == null)
            {
                mfaSetting = new UserMFASetting
                {
                    ApplicationUserId = user.Id,
                    MFAType = "AuthenticatorApp",
                    IsMFAEnabled = false,
                    CreatedAt = DateTime.UtcNow
                };
                await _db.UserMFASettings.AddAsync(mfaSetting);
            }

            if (string.IsNullOrEmpty(mfaSetting.SecretKey))
            {
                mfaSetting.SecretKey = _mfaSecurity.GenerateRandomSecretKey();
                var backupCodesList = _mfaSecurity.GenerateBackupCodes();
                mfaSetting.BackupCodes = string.Join(",", backupCodesList);
            }

            mfaSetting.UpdatedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync();

            string keyUri = _mfaSecurity.GenerateQrCodeUri(user.Email!, mfaSetting.SecretKey);

            using (var qrGenerator = new QRCoder.QRCodeGenerator())
            using (var qrCodeData = qrGenerator.CreateQrCode(keyUri, QRCoder.QRCodeGenerator.ECCLevel.Q))
            using (var qrCode = new QRCoder.PngByteQRCode(qrCodeData))
            {
                byte[] qrCodeBytes = qrCode.GetGraphic(20);
                string base64Image = $"data:image/png;base64,{Convert.ToBase64String(qrCodeBytes)}";

                var freshPreAuthTokenResult = _tokenService.GeneratePreAuthToken(user);

                return new SetupMfaResponse
                {
                    Succeeded = true,
                    RequiresMfaSetup = true,
                    QrCodeBase64 = base64Image,
                    SharedSecret = mfaSetting.SecretKey,
                    PreAuthToken = freshPreAuthTokenResult.Token
                };
            }
        }

        public async Task<bool> VerifyAndTrackMfaAppCodeAsync(string userId, string code)
        {
            try
            {
                if (await _accountLockService.IsAccountLockedAsync(userId))
                {
                    return false;
                }

                var secretKey = await GetUserMfaSecretKeyAsync(userId);
                if (string.IsNullOrEmpty(secretKey)) return false;

                bool isValid = _mfaSecurity.ValidateAuthenticatorCode(secretKey, code);
                if (!isValid)
                {
                    await _accountLockService.IncrementFailedAttemptsAsync(userId, 5, 30);
                    return false;
                }

                await _accountLockService.ResetFailedAttemptsAsync(userId);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[MFA APP CODE VERIFICATION EXCEPTION]: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> VerifyAndTrackEmailOtpAsync(string userId, string code)
        {
            try
            {
                if (await _accountLockService.IsAccountLockedAsync(userId))
                {
                    return false;
                }

                bool isValid = await _otpService.VerifyOtpAsync(userId, code, "Login");
                if (!isValid)
                {
                    await _accountLockService.IncrementFailedAttemptsAsync(userId, 5, 30);
                    return false;
                }

                await _accountLockService.ResetFailedAttemptsAsync(userId);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[MFA EMAIL OTP VERIFICATION EXCEPTION]: {ex.Message}");
                return false;
            }
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

        private async Task SendEmailChallengeAsync(string email, string? firstName, string otpCode)
        {
            string greetingName = string.IsNullOrWhiteSpace(firstName) ? "User" : firstName;
            string subject = "Online Evaluation System - Login Verification Code";

            string body = $@"
                <div style='font-family: Arial, sans-serif; padding: 20px; border: 1px solid #e2e8f0; border-radius: 8px; max-width: 500px;'>
                    <h2 style='color: #2b6cb0; margin-top: 0;'>Security Verification</h2>
                    <p>Hello <strong>{greetingName}</strong>,</p>
                    <p>A login request was initiated for your account. Use the following security code to finalize authentication:</p>
                    <div style='text-align: center; margin: 25px 0;'>
                        <span style='font-size: 24px; font-weight: bold; letter-spacing: 4px; padding: 10px 20px; background-color: #edf2f7; border-radius: 4px; color: #2d3748;'>{otpCode}</span>
                    </div>
                    <p style='font-size: 13px; color: #718096;'>This challenge token will expire in 5 minutes. If you did not make this request, please change your credentials immediately.</p>
                </div>";

            await _emailQueue.QueueEmailAsync(new EmailJob
            {
                ToEmail = email,
                Subject = subject,
                Body = body
            });
        }


    }
}
