namespace OnlineEvaluation.Api.Models.DTO
{
    public class LoginResult
    {
        public bool Succeeded { get; set; }
        public bool IsLockedOut { get; set; }
        public bool IsMfaRequired { get; set; }
        public bool RequiresPasswordChange { get; set; }


        public string? AccessToken { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime? AccessTokenExpiresAt { get; set; }


        public string? PreAuthToken { get; set; }
        public string? MfaType { get; set; }
    }
}
