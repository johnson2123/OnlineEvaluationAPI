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

        public AccountController(IAuthService auth, IConfiguration configuration)
        {
            _auth = auth;
            _configuration = configuration;
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
                var clientMetadata = GetClientMetadata();
                var authResponse = await _auth.LoginAsync(dto, clientMetadata);

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
                    string distinctMessage = authResponse.MfaType switch
                    {
                        "Email" => "A secure verification code has been sent to your registered email address.",
                        "AuthenticatorApp" => "Please enter the active 6-digit code from your Authenticator App.",
                        _ => "Multi-Factor Authentication code verification is required."
                    };
                    return Accepted(new
                    {
                        isMfaRequired = true,
                        preAuthToken = authResponse.PreAuthToken,
                        message = distinctMessage
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

                bool isValid = await _auth.VerifyAndTrackMfaAppCodeAsync(userId, dto.Code);
                if (!isValid)
                {
                    return Unauthorized(new { error = "Invalid verification code or account is temporarily locked out." });
                }

                await _auth.ActivateMfaAsync(userId);

                var clientMetadata = GetClientMetadata();
                var finalAuthResponse = await _auth.GenerateFinalLoginTokensAsync(userId, clientMetadata);

                AppendRefreshTokenCookie(finalAuthResponse.RefreshToken);

                return Ok(new
                {
                    accessToken = finalAuthResponse.AccessToken,
                    expiresAt = finalAuthResponse.AccessTokenExpiresAt
                });
            }
            catch (Exception)
            {
                return StatusCode(500, new { error = "An internal error occurred during verification." });
            }
        }

        [AllowAnonymous]
        [HttpPost("verify-email-otp")]
        public async Task<IActionResult> VerifyEmailOtp([FromBody] MfaVerificationDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                var userId = _auth.ValidatePreAuthToken(dto.PreAuthToken);
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new { error = "Session expired or token invalid. Please log in again." });
                }

                bool isOtpValid = await _auth.VerifyAndTrackEmailOtpAsync(userId, dto.Code);
                if (!isOtpValid)
                {
                    return Unauthorized(new { error = "Invalid/expired verification code or account is temporarily locked out." });
                }

                await _auth.ActivateMfaAsync(userId);

                var clientMetadata = GetClientMetadata();
                var finalAuthResponse = await _auth.GenerateFinalLoginTokensAsync(userId, clientMetadata);

                AppendRefreshTokenCookie(finalAuthResponse.RefreshToken);

                return Ok(new
                {
                    accessToken = finalAuthResponse.AccessToken,
                    expiresAt = finalAuthResponse.AccessTokenExpiresAt
                });
            }
            catch (Exception)
            {
                return StatusCode(500, new { error = "An internal error occurred during email verification." });
            }
        }

        [AllowAnonymous]
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

        private ClientMetadataDto GetClientMetadata()
        {
            string ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";

            if (Request.Headers.TryGetValue("X-Forwarded-For", out var forwardedFor))
            {
                ipAddress = forwardedFor.ToString().Split(',')[0].Trim();
            }

            string userAgent = Request.Headers["User-Agent"].ToString() ?? "Unknown";

            return new ClientMetadataDto
            {
                IPAddress = ipAddress,
                BrowserInfo = userAgent,
                OperatingSystem = "Extracted from User-Agent", 
                DeviceInfo = "Extracted from User-Agent"
            };
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

        [AllowAnonymous]
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            bool isDispatched = await _auth.ForgotPasswordAsync(dto.Email);

            if (!isDispatched)
            {
                return StatusCode(500, new
                {
                    error = "The system encountered an error while processing your recovery request. Please try again later."
                });
            }

            return Ok(new { message = "If the email matches an active account, a password recovery link has been dispatched." });
        }

        [AllowAnonymous]
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                bool isResetSuccessful = await _auth.ResetPasswordAsync(dto);

                if (!isResetSuccessful)
                {
                    return BadRequest(new
                    {
                        error = "Unable to reset password. The recovery token may be invalid, expired, or the new password does not meet requirements."
                    });
                }

                return Ok(new { message = "Password reset successfully. You may now log in with your new credentials." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = $"An internal server error occurred while processing your request: {ex.Message}" });
            }
        }

        [HttpPost("request-password-change")]
        [Authorize]
        public async Task<IActionResult> RequestPasswordChange()
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new { error = "User identity context could not be parsed from token session." });
            }

            bool isSent = await _auth.SendPasswordChangeOtpAsync(userId);

            if (!isSent)
            {
                return BadRequest(new { error = "Unable to initiate password change. Profile may not have a valid email configured." });
            }

            return Ok(new { message = "A unique 6-digit security verification code has been forwarded to your registered email." });
        }

        [HttpPost("password-change")]
        [Authorize]
        public async Task<IActionResult> PasswordChange([FromBody] PasswordChangeDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            try
            {
                var result = await _auth.PasswordChangeAsync(userId, dto);

                if (!result.Succeeded)
                {
                    return BadRequest(new { error = result.Errors.FirstOrDefault()?.Description });
                }

                return Ok(new { message = "Your account password has been updated successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }
    }
}
