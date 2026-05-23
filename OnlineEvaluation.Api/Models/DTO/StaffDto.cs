namespace OnlineEvaluation.Api.Models.DTO
{
    public class StaffDto
    {
        public int Id { get; set; }
        public Guid StaffGuid { get; set; }
        public string Email { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string CollegeDepartmentAliasCode { get; set; } = string.Empty;
        public string CollegeDepartmentName { get; set; } = string.Empty;
        public string EmployeeId { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string Designation { get; set; } = string.Empty;
        public string Gender { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public bool IsTeachingStaff { get; set; }
        public bool IsPermanent { get; set; }
        public string HighestQualification { get; set; } = string.Empty;
        public int? ReportsToProfileId { get; set; }
        public string? ReportsToStaffName { get; set; }
        public string Address { get; set; } = string.Empty;
    }
}
