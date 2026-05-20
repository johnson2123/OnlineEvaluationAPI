namespace OnlineEvaluation.Api.Models.DTO
{
    public class ExamSpecDto
    {
        public int Id { get; set; }
        public Guid Guid { get; set; }
        public string ExamSpecCode { get; set; } = string.Empty;
        public int AcademicMapId { get; set; }

        public string Regulation { get; set; } = string.Empty;
        public int SubjectId { get; set; }
        public string SubjectCode { get; set; } = string.Empty;
        public string SubjectName { get; set; } = string.Empty;
        public bool IsElective { get; set; }

        public int Semester { get; set; }
        public int InternalMaxMarks { get; set; }
        public int ExternalMaxMarks { get; set; }
        public int TotalMaxMarks { get; set; }
        public int ExternalPassingMarks { get; set; }
        public int TotalPassingMarks { get; set; }

        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
    }
}
