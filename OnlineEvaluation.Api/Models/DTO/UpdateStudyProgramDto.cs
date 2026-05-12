using OnlineEvaluation.Api.Constants;

namespace OnlineEvaluation.Api.Models.DTO
{
    public class UpdateStudyProgramDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string ShortName { get; set; } = string.Empty;
        public AcademicLevel Level { get; set; }
        public int DurationInYears { get; set; }
        public int TotalSemesters { get; set; }
        public bool IsActive { get; set; }
    }
}
