namespace OnlineEvaluation.Api.Models.Entities
{
    public class StaffProfile
    {
        public int Id { get; set; }
        public Guid StaffGuid { get; set; } = Guid.NewGuid();


        public string ApplicationUserId { get; set; } = string.Empty;
        public virtual ApplicationUser ApplicationUser { get; set; } = null!;


        public int? CollegeDepartmentId { get; set; }
        public string CollegeDepartmentAliasCode { get; set; } = string.Empty;
        public virtual CollegeDepartment CollegeDepartment { get; set; } = null!;


        public string EmployeeId { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty; // e.g., "Controller", "Moderator", "Faculty"
        public string Designation { get; set; } = string.Empty; 
        public string Gender { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }


        public bool IsTeachingStaff { get; set; }
        public bool IsPermanent { get; set; }
        public string HighestQualification { get; set; } = string.Empty;


        public int? ReportsToProfileId { get; set; }
        public virtual StaffProfile? ReportsToProfile { get; set; }


        public string Address { get; set; } = string.Empty;


        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string CreatedBy { get; set; } = string.Empty;
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }

        public bool IsDeleted { get; set; } = false;
        public DateTime? DeletedAt { get; set; }
        public string? DeletedBy { get; set; }

        public string FullName => ApplicationUser != null ? ApplicationUser.FullName : string.Empty;
    }
}
