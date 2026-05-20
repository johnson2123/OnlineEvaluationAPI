namespace OnlineEvaluation.Api.Models.DTO
{
    public class CreateExamSpecDto
    {
        public int AcademicMapId { get; set; }
        public int SubjectId { get; set; }
        public int Semester { get; set; }
        public int InternalMaxMarks { get; set; }
        public int ExternalMaxMarks { get; set; }
        public int TotalMaxMarks { get; set; }
        public int ExternalPassingMarks { get; set; }
        public int TotalPassingMarks { get; set; }
    }
}
