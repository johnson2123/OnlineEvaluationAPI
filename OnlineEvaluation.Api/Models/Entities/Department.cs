namespace OnlineEvaluation.Api.Models.Entities
{
    public class Department
    {
        public int Id { get; set; }
        public Guid Guid { get; set; } = Guid.NewGuid();

        public string Code { get; set; } = string.Empty; // "CSE", "ECE"
        public string Name { get; set; } = string.Empty; // "Computer Science & Engineering"
        public string? DisplayName { get; set; }
        public string? Description { get; set; }
        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string CreatedBy { get; set; } = string.Empty;
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }

        public bool IsDeleted { get; set; } = false;
        public DateTime? DeletedAt { get; set; }
        public string? DeletedBy { get; set; }


        //public virtual ICollection<CollegeDepartmentMapping> CollegeDepartmentMappings { get; set; } = new HashSet<CollegeDepartmentMapping>();
    }
}
