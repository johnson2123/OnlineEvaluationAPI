using OnlineEvaluation.Api.Constants;

namespace OnlineEvaluation.Api.Models.Entities
{
    public class Subject
    {
        public int Id { get; set; }
        public Guid Guid { get; set; } = Guid.NewGuid();

        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? DisplayName { get; set; }
        public string? Description { get; set; }

        public int Credits { get; set; }
        public SubjectType Type { get; set; } = SubjectType.Theory;

        public bool IsElective { get; set; }

        public bool IsActive { get; set; } = true;


        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }
        public bool IsDeleted { get; set; } = false;
        public DateTime? DeletedAt { get; set; }
        public string? DeletedBy { get; set; }
    }
}
