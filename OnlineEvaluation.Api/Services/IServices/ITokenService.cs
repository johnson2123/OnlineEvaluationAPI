using OnlineEvaluation.Api.Models;
using OnlineEvaluation.Api.Services.Helpers;

namespace OnlineEvaluation.Api.Services.IServices
{
    public interface ITokenService
    {
        TokenResult GenerateAccessToken(ApplicationUser user, IEnumerable<string> roles);
        string GenerateRefreshToken();

    }
}
