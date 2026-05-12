using OnlineEvaluation.Api.Constants;

namespace OnlineEvaluation.Api.Models.DTO
{
    public class CreateStudyProgramDto
    {
        public string Name { get; set; } = string.Empty;
        public string ShortName { get; set; } = string.Empty;
        public AcademicLevel Level { get; set; }
        public int DurationInYears { get; set; }
        public int TotalSemesters { get; set; }
    }
}
