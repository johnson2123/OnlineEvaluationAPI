namespace OnlineEvaluation.Api.Models.DTO
{
    public class StaffRegistrationDto
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string EmployeeId { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Password { get; set; }
        public int CollegeDepartmentId { get; set; }
        public string Role { get; set; } = string.Empty;
        public string Designation { get; set; } = string.Empty;
        public string Gender { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public bool IsTeachingStaff { get; set; }
        public bool IsPermanent { get; set; }
        public string HighestQualification { get; set; } = string.Empty;
        public int? ReportsToProfileId { get; set; }
        public string Address { get; set; } = string.Empty;
    }
}
