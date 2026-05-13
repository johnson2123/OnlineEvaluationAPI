namespace OnlineEvaluation.Api.Models.Entities
{
    public class AcademicMap
    {
        public int Id { get; set; }
        public Guid Guid { get; set; }


        public int CollegeId { get; set; }
        public int StudyProgramId { get; set; }
        public int BranchId { get; set; }


        // This will hold the "AUCE-BTECH-CSE" logic
        public string? AliasCode { get; set; }
        public bool IsActive { get; set; }


        public DateTime CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }


        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }
        public string? DeletedBy { get; set; }


        public virtual College College { get; set; } = null!;
        public virtual StudyProgram StudyProgram { get; set; } = null!;
        public virtual Branch Branch { get; set; } = null!;


        //public virtual ICollection<SubjectExamConfig> SubjectExamConfigs { get; set; } = new List<SubjectExamConfig>();

        //public virtual ICollection<Student> Students { get; set; } = new List<Student>();
    }
}
