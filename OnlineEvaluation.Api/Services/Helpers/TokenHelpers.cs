using System.Security.Cryptography;
using System.Text;

namespace OnlineEvaluation.Api.Services.Helpers
{
    public class TokenHelpers
    {
        public static string ComputeSha256HashBase64(string token)
        {
            using var sha = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(token);
            var hash = sha.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }

        public static string ComputeHmacSha256Base64(string token, string secretKey)
        {
            if (string.IsNullOrEmpty(secretKey))
                return ComputeSha256HashBase64(token);

            var keyBytes = Encoding.UTF8.GetBytes(secretKey);
            using var hmac = new HMACSHA256(keyBytes);
            var bytes = Encoding.UTF8.GetBytes(token);
            var hash = hmac.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }
    }
}
