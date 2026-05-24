using OnlineEvaluation.Api.Models;
using OnlineEvaluation.Api.Services.Helpers;

namespace OnlineEvaluation.Api.Services.IServices
{
    public interface ITokenService
    {
        TokenResult GeneratePreAuthToken(ApplicationUser user);
        TokenResult GenerateAccessToken(ApplicationUser user, IEnumerable<string> roles);
        string GenerateRefreshToken();
        string ValidateTokenAndGetUserId(string token);

    }
}
