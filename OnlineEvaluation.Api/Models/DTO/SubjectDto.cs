namespace OnlineEvaluation.Api.Models.DTO
{
    public class SubjectDto
    {
        public int Id { get; set; }
        public Guid Guid { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? DisplayName { get; set; }
        public string? Description { get; set; }
        public int Credits { get; set; }
        public int Type { get; set; } // Enum value
        public string TypeName { get; set; } = string.Empty; // "Theory", "Practical", etc.
        public bool IsElective { get; set; }
        public bool IsActive { get; set; }
    }
}
