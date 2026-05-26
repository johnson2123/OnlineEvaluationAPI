using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using OnlineEvaluation.Api.Models.DTO;
using OnlineEvaluation.Api.Services;
using OnlineEvaluation.Api.Services.IServices;

namespace OnlineEvaluation.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAuthService _auth;
        private readonly IConfiguration _configuration;
        private readonly IMfaSecurityService _mfaSecurity;
        private readonly IOtpService _otpService;

        public AccountController(IAuthService auth, IConfiguration configuration, IMfaSecurityService mfaSecurity, IOtpService otpService)
        {
            _auth = auth;
            _configuration = configuration;
            _mfaSecurity = mfaSecurity;
            _otpService = otpService;
        }

        [HttpPost("register")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _auth.RegisterAsync(dto);

            if (!result.Succeeded)
            {
                return BadRequest(new { errors = result.Errors });
            }

            return Ok(new { message = "Registration Successful" });

        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var authResponse = await _auth.LoginAsync(dto);

                if (authResponse.RequiresPasswordChange)
                {
                    return Ok(new
                    {
                        requiresPasswordChange = true,
                        message = "First-time login. Please redirect to password setup."
                    });
                }

                if (authResponse.RequiresMfaSetup)
                {
                    return Accepted(new
                    {
                        requiresMfaSetup = true,
                        preAuthToken = authResponse.PreAuthToken,
                        message = "MFA Setup has been initialized but is incomplete. Please proceed via setup verification."
                    });
                }

                if (authResponse.IsMfaRequired)
                {
                    return Accepted(new
                    {
                        isMfaRequired = true,
                        preAuthToken = authResponse.PreAuthToken,
                        message = "MFA code verification required."
                    });
                }

                if (string.IsNullOrEmpty(authResponse.RefreshToken))
                {
                    return Unauthorized(new { error = "Authentication state is invalid." });
                }

                AppendRefreshTokenCookie(authResponse.RefreshToken);

                return Ok(new
                {
                    accessToken = authResponse.AccessToken,
                    expiresAt = authResponse.AccessTokenExpiresAt
                });
            }
            catch(UnauthorizedAccessException ex)
            {
                return Unauthorized(new { error = ex.Message });
            }
        }

        [AllowAnonymous]
        [HttpPost("verify-mfa")]
        public async Task<IActionResult> VerifyMfa([FromBody] MfaVerificationDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                var userId = _auth.ValidatePreAuthToken(dto.PreAuthToken);

                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new { error = "Session expired or invalid. Please re-enter your password." });
                }

                var userSecretKey = await _auth.GetUserMfaSecretKeyAsync(userId);
                if (string.IsNullOrEmpty(userSecretKey))
                {
                    return BadRequest(new { error = "MFA configuration not found for this account." });
                }

                bool isValid = _mfaSecurity.ValidateAuthenticatorCode(userSecretKey, dto.Code);
                if (!isValid)
                {
                    return Unauthorized(new { error = "Invalid verification code. Please try again." });
                }

                // Activating MFA if it's the user's initial setup phase
                await _auth.ActivateMfaAsync(userId);

                var finalAuthResponse = await _auth.GenerateFinalLoginTokensAsync(userId);

                AppendRefreshTokenCookie(finalAuthResponse.RefreshToken);

                return Ok(new
                {
                    accessToken = finalAuthResponse.AccessToken,
                    expiresAt = finalAuthResponse.AccessTokenExpiresAt
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "An internal error occurred during verification." });
            }
        }

        [AllowAnonymous]
        [HttpPost("verify-email-otp")]
        public async Task<IActionResult> VerifyEmailOtp([FromBody] MfaVerificationDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var userId = _auth.ValidatePreAuthToken(dto.PreAuthToken);

                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new { error = "Session expired or token invalid. Please log in again." });
                }

                bool isOtpValid = await _otpService.VerifyOtpAsync(userId, dto.Code, "Login");

                if (!isOtpValid)
                {
                    return Unauthorized(new { error = "Invalid or expired verification code." });
                }

                await _auth.ActivateMfaAsync(userId);

                var finalAuthResponse = await _auth.GenerateFinalLoginTokensAsync(userId);

                AppendRefreshTokenCookie(finalAuthResponse.RefreshToken);

                return Ok(new
                {
                    accessToken = finalAuthResponse.AccessToken,
                    expiresAt = finalAuthResponse.AccessTokenExpiresAt
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "An internal error occurred during email verification." });
            }
        }

        [HttpPost("setup-password")]
        public async Task<IActionResult> SetupPassword([FromBody] ChangeInitialPasswordDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                var result = await _auth.ChangeInitialPasswordAsync(dto);

                if (!result.Succeeded)
                {
                    return BadRequest(new { errors = result.Errors });
                }

                if (result.RequiresMfaSetup)
                {
                    return Ok(new
                    {
                        requiresMfaSetup = true,
                        preAuthToken = result.PreAuthToken,
                        message = "Password updated successfully. A verification code has been sent to your email. Please verify it to view your Authenticator QR Code."
                    });
                }

                return Ok(new
                {
                    requiresMfaSetup = false,
                    message = "Account verified and password updated successfully."
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = $"An internal error occurred: {ex.Message}" });
            }
        }

        [AllowAnonymous]
        [HttpPost("verify-onboarding-otp")]
        public async Task<IActionResult> VerifyOnboardingOtp([FromBody] VerifyMfaOnboardingOtpDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var result = await _auth.VerifyMfaOnboardingOtpAsync(dto);

                if (!result.Succeeded)
                {
                    return BadRequest(new { errors = result.Errors });
                }

                return Ok(new
                {
                    requiresMfaSetup = true,
                    qrCodeBase64 = result.QrCodeBase64,
                    sharedSecret = result.SharedSecret,
                    preAuthToken = result.PreAuthToken,
                    message = "Email verified successfully. Please scan this QR code with an authenticator app like Google Authenticator or Microsoft Authenticator, then enter the device code to activate."
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = $"An unexpected error occurred during OTP onboarding validation: {ex.Message}" });
            }
        }


        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh()
        {
            if (!Request.Cookies.TryGetValue("refreshToken", out var refreshToken) || string.IsNullOrEmpty(refreshToken))
            {
                return Unauthorized(new { error = "Refresh token missing" });
            }
            try
            {
                var authResponse = await _auth.RefreshTokenAsync(refreshToken);

                AppendRefreshTokenCookie(authResponse.RefreshToken);

                return Ok(new
                {
                    accessToken = authResponse.AccessToken,
                    expiresAt = authResponse.AccessTokenExpiresAt
                });
            }
            catch (UnauthorizedAccessException)
            {
                RemoveRefreshTokenCookie();
                return Unauthorized(new { error = "Invalid or expired session. Please login again." });
            }
        }

        [HttpPost("revoke")]
        [Authorize]
        public async Task<IActionResult> Revoke()
        {
           
            if (Request.Cookies.TryGetValue("refreshToken", out var refreshToken) && !string.IsNullOrEmpty(refreshToken))
            {
                await _auth.RevokeRefreshTokenAsync(refreshToken);
            }

            RemoveRefreshTokenCookie();

            return Ok(new { message = "Logged out successfully" });
        }

        // NEW: This centralizes the cookie creation logic so Login and Refresh use the exact same settings.
        private void AppendRefreshTokenCookie(string token)
        {
            var expiryMinutes = Convert.ToDouble(_configuration["Jwt:RefreshTokenExpireMinutes"] ?? "60");
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = DateTime.UtcNow.AddMinutes(expiryMinutes),
                Path = "/"
            };

            Response.Cookies.Append("refreshToken", token, cookieOptions);
        }

        // NEW: This centralizes cookie deletion, ensuring the Path and SameSite settings match the creation settings.
        private void RemoveRefreshTokenCookie()
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = DateTime.UtcNow.AddDays(-1), // Expire immediately
                Path = "/"
            };

            Response.Cookies.Delete("refreshToken", cookieOptions);
        }



        // Email endpoints intentionally return 400 in dev to avoid confusion
        [HttpPost("confirm-email")]
        public IActionResult ConfirmEmail() => BadRequest(new { error = "Email confirmation disabled in dev." });

        [HttpPost("forgot-password")]
        public IActionResult ForgotPassword() => BadRequest(new { error = "Forgot password disabled in dev." });

        [HttpPost("reset-password")]
        public IActionResult ResetPassword() => BadRequest(new { error = "Reset password disabled in dev." });
    }
}
