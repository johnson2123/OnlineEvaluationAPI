namespace OnlineEvaluation.Api.Models.DTO
{
    public class StaffProfileDto
    {
        public Guid StaffGuid { get; set; }
        public string EmployeeId { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;


        public string? CollegeName { get; set; }
        public string? DepartmentName { get; set; } // We will fetch the readable name here
        public string Role { get; set; } = string.Empty;
        public string Designation { get; set; } = string.Empty;
        public string Gender { get; set; } = string.Empty;
        public string HighestQualification { get; set; } = string.Empty;
        public bool IsPermanent { get; set; }


        public string? PhoneNumber { get; set; }
        public string Address { get; set; } = string.Empty;
    }
}
