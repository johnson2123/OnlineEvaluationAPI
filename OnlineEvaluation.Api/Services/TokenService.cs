using Microsoft.IdentityModel.Tokens;
using OnlineEvaluation.Api.Models;
using OnlineEvaluation.Api.Services.Helpers;
using OnlineEvaluation.Api.Services.IServices;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace OnlineEvaluation.Api.Services
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _config;
        private readonly int _accessTokenMinutes;
        private readonly string _fallbackIssuer = "OnlineEvaluationApp";
        private readonly string _fallbackAudience = "OnlineEvaluationAppUsers";

        public TokenService(IConfiguration config)
        {
            _config = config;
            _accessTokenMinutes = Convert.ToInt32(_config["Jwt:AccessTokenExpireMinutes"] ?? "15");

            var testRead = _config["Jwt:Issuer"];
            Console.WriteLine($"[CONSTRUCTOR CHECK] TokenService created. Read Issuer: '{testRead}'");
        }

        public TokenResult GeneratePreAuthToken(ApplicationUser user)
        {
            DateTime nowUtc = DateTime.UtcNow;
            DateTime expiryUtc = nowUtc.AddMinutes(15);

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                new Claim(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty),
                new Claim("fullname", user.FullName ?? string.Empty),
                new Claim(ClaimTypes.Role, "MfaChallengePending"),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                notBefore: nowUtc,
                expires: expiryUtc,
                signingCredentials: creds
            );

            return new TokenResult
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                ExpiresAtUtc = expiryUtc
            };
        }
        public TokenResult GenerateAccessToken(ApplicationUser user, IEnumerable<string> roles)
        {

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id), 
                new Claim(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty), 
                // ===========================
                new Claim("fullname", user.FullName ?? string.Empty),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var expiry = DateTime.UtcNow.AddMinutes(_accessTokenMinutes);

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: expiry,
                signingCredentials: creds
                );

            return new TokenResult
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                ExpiresAtUtc = expiry
            };
        }

        public string GenerateRefreshToken()
        {
            var randomBytes = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomBytes);
            return Convert.ToBase64String(randomBytes);
        }

        public string ValidateTokenAndGetUserId(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
                return string.Empty;

            string expectedIssuer = _config["Jwt:Issuer"]?.Trim() ?? _fallbackIssuer;
            string expectedAudience = _config["Jwt:Audience"]?.Trim() ?? _fallbackAudience;
            string secretKey = _config["Jwt:Key"]?.Trim();

            if (string.IsNullOrWhiteSpace(secretKey))
                return string.Empty;

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(secretKey);

            try
            {
                ClaimsPrincipal principal = tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),

                    ValidateIssuer = true,
                    IssuerValidator = (issuer, securityToken, validationParameters) =>
                    {
                        if (string.IsNullOrEmpty(issuer) || issuer == expectedIssuer)
                        {
                            return expectedIssuer; // Return the valid issuer to satisfy the engine
                        }
                        throw new SecurityTokenInvalidIssuerException("Issuer validation failed.");
                    },

                    ValidateAudience = true,
                    ValidAudience = expectedAudience,

                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.FromSeconds(5)
                }, out _);

                var userIdClaim = principal.FindFirst(JwtRegisteredClaimNames.Sub)
                                  ?? principal.FindFirst(ClaimTypes.NameIdentifier);

                return userIdClaim?.Value ?? string.Empty;
            }
            catch
            {
                return string.Empty;
            }
        }
    }
}
