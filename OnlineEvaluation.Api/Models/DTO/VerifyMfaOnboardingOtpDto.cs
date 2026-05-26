namespace OnlineEvaluation.Api.Models.DTO
{
    public class VerifyMfaOnboardingOtpDto
    {
        public string Email { get; set; } = string.Empty;
        public string OtpCode { get; set; } = string.Empty;
        public string PreAuthToken { get; set; } = string.Empty;
    }
}
