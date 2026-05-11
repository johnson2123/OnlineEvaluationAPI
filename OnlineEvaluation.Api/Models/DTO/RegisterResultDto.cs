namespace OnlineEvaluation.Api.Models.DTO
{
    public class RegisterResultDto
    {
        public bool Succeeded { get; set; }
        public string[] Errors { get; set; } = [];

        public RegisterResultDto() { }

        public RegisterResultDto(bool succeeded, IEnumerable<string> errors)
        {
            Succeeded = succeeded;
            Errors = errors?.ToArray() ?? Array.Empty<string>();
        }
    }
}
