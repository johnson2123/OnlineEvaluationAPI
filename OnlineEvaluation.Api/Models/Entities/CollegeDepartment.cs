namespace OnlineEvaluation.Api.Models.Entities
{
    public class CollegeDepartment
    {
        public int Id { get; set; }
        public Guid Guid { get; set; } = Guid.NewGuid();

        public string AliasCode { get; set; } = string.Empty;

        public int CollegeId { get; set; }
        public int DepartmentId { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string CreatedBy { get; set; } = string.Empty;
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }

        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }
        public string? DeletedBy { get; set; }

        public virtual College College { get; set; } = null!;
        public virtual Department Department { get; set; } = null!;
    }
}
