namespace OnlineEvaluation.Api.Models.DTO
{
    public class MfaVerificationDto
    {
        public string PreAuthToken { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
    }
}
