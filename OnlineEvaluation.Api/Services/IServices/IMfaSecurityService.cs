namespace OnlineEvaluation.Api.Services.IServices
{
    public interface IMfaSecurityService
    {
        string GenerateRandomSecretKey();
        string GenerateQrCodeUri(string userEmail, string secretKey);
        bool ValidateAuthenticatorCode(string secretKey, string inputCode);
        List<string> GenerateBackupCodes();
    }
}
