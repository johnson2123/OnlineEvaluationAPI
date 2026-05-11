namespace OnlineEvaluation.Api.Services.Helpers
{
    public class TokenResult
    {
        public string Token { get; set; } = string.Empty;
        public DateTime ExpiresAtUtc { get; set; }

    }
}
