using OnlineEvaluation.Api.Constants;

namespace OnlineEvaluation.Api.Models.DTO
{
    public class UpdateSubjectDto
    {
        public int Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? DisplayName { get; set; }
        public string? Description { get; set; }
        public int Credits { get; set; }
        public SubjectType Type { get; set; }
        public bool IsElective { get; set; }
        public bool IsActive { get; set; }
    }
}
