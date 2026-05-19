namespace OnlineEvaluation.Api.Models.Entities
{
    public class Student
    {
        public int Id { get; set; }
        public Guid Guid { get; set; } = Guid.NewGuid();
        public string ApplicationUserId { get; set; }
        public virtual ApplicationUser User { get; set; }
        public string RegistrationNumber { get; set; }
        public string Batch { get; set; }
        public string AcademicAliasCode { get; set; }
        public int AcademicMapId { get; set; }
        public virtual AcademicMap AcademicMap { get; set; }
        public bool IsActive { get; set; }
        public string FatherName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Gender { get; set; }
        public string? ContactNumber { get; set; }
        public string? Address { get; set; }
        public string? BloodGroup { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }


        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }
        public string? DeletedBy { get; set; }


        public virtual ICollection<StudentAcademicRecord> StudentAcademicRecords { get; set; } = new List<StudentAcademicRecord>();
    }
}
