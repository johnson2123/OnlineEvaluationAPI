namespace OnlineEvaluation.Api.Models.Entities
{
    public class ExamCodeSpecification
    {
        public int Id { get; set; }
        public Guid Guid { get; set; } = Guid.NewGuid();
        public string ExamSpecCode { get; set; } = string.Empty; //AUCEW-BTECH-CSE-R22-SEM3-CS411
        public int AcademicMapId { get; set; }
        public virtual AcademicMap AcademicMap { get; set; } = null!;
        public int SubjectId { get; set; }
        public virtual Subject Subject { get; set; } = null!;

        public int Semester { get; set; }
        public int InternalMaxMarks { get; set; }
        public int ExternalMaxMarks { get; set; } 
        public int TotalMaxMarks { get; set; }    


        public int ExternalPassingMarks { get; set; } 
        public int TotalPassingMarks { get; set; }   


        public bool IsActive { get; set; } = true;


        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string CreatedBy { get; set; } = string.Empty;
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }


        public bool IsDeleted { get; set; } = false;
        public DateTime? DeletedAt { get; set; }
        public string? DeletedBy { get; set; }
    }
}
