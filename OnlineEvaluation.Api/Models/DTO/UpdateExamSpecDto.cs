namespace OnlineEvaluation.Api.Models.DTO
{
    public class UpdateExamSpecDto
    {
        public int InternalMaxMarks { get; set; }
        public int ExternalMaxMarks { get; set; }
        public int TotalMaxMarks { get; set; }
        public int ExternalPassingMarks { get; set; }
        public int TotalPassingMarks { get; set; }
        public bool IsActive { get; set; }
    }
}
