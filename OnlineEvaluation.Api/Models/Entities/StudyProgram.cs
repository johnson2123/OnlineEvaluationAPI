using OnlineEvaluation.Api.Constants;

namespace OnlineEvaluation.Api.Models.Entities
{
    public class StudyProgram
    {
        public int Id { get; set; }
        public Guid Guid { get; set; } = Guid.NewGuid();

        public string Name { get; set; } = string.Empty;
        public string ShortName { get; set; } = string.Empty;
        public AcademicLevel Level { get; set; }

        public int DurationInYears { get; set; }
        public int TotalSemesters { get; set; }

        public bool IsActive { get; set; } = true;


        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string? CreatedByUserId { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedByUserId { get; set; }
        public bool IsDeleted { get; set; } = false;
        public DateTime? DeletedAt { get; set; }
        public string? DeletedByUserId { get; set; }
    }
}
