using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using OnlineEvaluation.Api.Models.DTO;
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

                AppendRefreshTokenCookie(authResponse.RefreshToken, authResponse.AccessTokenExpiresAt);

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

                AppendRefreshTokenCookie(authResponse.RefreshToken, authResponse.AccessTokenExpiresAt);

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
            if (!Request.Cookies.TryGetValue("refreshToken", out var refreshToken) || string.IsNullOrEmpty(refreshToken))
            {
                return BadRequest(new { error = "Refresh token missing" });
            }
                
            var success = await _auth.RevokeRefreshTokenAsync(refreshToken);

            if (!success)
            {
                return NotFound(new { error = "Refresh token not found or already revoked" });
            }

            // Clear cookie
            RemoveRefreshTokenCookie();

            return Ok(new { message = "Refresh token revoked and cookie cleared" });
        }

        // NEW: This centralizes the cookie creation logic so Login and Refresh use the exact same settings.
        private void AppendRefreshTokenCookie(string token, DateTime accessTokenExpiresAt)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = accessTokenExpiresAt.AddDays(Convert.ToDouble(_configuration["Jwt:RefreshTokenExpireDays"] ?? "30")),
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

            Response.Cookies.Append("refreshToken", string.Empty, cookieOptions);
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
