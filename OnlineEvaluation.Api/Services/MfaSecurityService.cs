using OnlineEvaluation.Api.Services.IServices;
using OtpNet;
using System.Security.Cryptography;
using System.Text.Encodings.Web;

namespace OnlineEvaluation.Api.Services
{
    public class MfaSecurityService : IMfaSecurityService
    {
        private const string IssuerName = "OnlineEvaluationSystem";
        public List<string> GenerateBackupCodes()
        {
            var codes = new List<string>();
            for (int i = 0; i < 8; i++)
            {
                byte[] randomBytes = new byte[4];
                RandomNumberGenerator.Fill(randomBytes);
                string code = BitConverter.ToString(randomBytes).Replace("-", "").Substring(0, 8).ToLower();
                codes.Add(code);
            }
            return codes;
        }

        public string GenerateQrCodeUri(string userEmail, string secretKey)
        {
            string encodedIssuer = UrlEncoder.Default.Encode(IssuerName);
            string encodedEmail = UrlEncoder.Default.Encode(userEmail);

            return $"otpauth://totp/{encodedIssuer}:{encodedEmail}?secret={secretKey}&issuer={encodedIssuer}&digits=6&period=30";
        }

        public string GenerateRandomSecretKey()
        {
            byte[] secretBytes = KeyGeneration.GenerateRandomKey(20); 
            return Base32Encoding.ToString(secretBytes);
        }

        public bool ValidateAuthenticatorCode(string secretKey, string inputCode)
        {
            if (string.IsNullOrWhiteSpace(secretKey) || string.IsNullOrWhiteSpace(inputCode))
                return false;

            try
            {
                string cleanedSecret = secretKey.Trim().Replace(" ", "").ToUpperInvariant();
                byte[] secretBytes = Base32Encoding.ToBytes(cleanedSecret);
                var totpEngine = new Totp(secretBytes);

                long timeStepMatched;
                bool isValid = totpEngine.VerifyTotp(
                    DateTime.UtcNow,
                    inputCode.Trim(),
                    out timeStepMatched,
                    new VerificationWindow(1,1)
                );

                return isValid;
            }
            catch
            {
                return false;
            }
        }
    }
}
