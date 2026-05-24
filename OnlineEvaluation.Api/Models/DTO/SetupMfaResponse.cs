namespace OnlineEvaluation.Api.Models.DTO
{
    public class SetupMfaResponse
    {
        public bool Succeeded { get; set; }
        public IEnumerable<string> Errors { get; set; } = Enumerable.Empty<string>();
        public bool RequiresMfaSetup { get; set; }
        public string? QrCodeBase64 { get; set; }
        public string? SharedSecret { get; set; }
        public string? PreAuthToken { get; set; }
    }
}
