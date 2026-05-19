namespace OnlineEvaluation.Api.Models.DTO
{
    public class StudentDto
    {
        public Guid Id { get; set; }
        public string ApplicationUserId { get; set; } = string.Empty;
        public string RegistrationNumber { get; set; } = string.Empty;
        public string Batch { get; set; } = string.Empty;
        public string AcademicAliasCode { get; set; } = string.Empty;

       
        public bool IsActive { get; set; }

        public string FatherName { get; set; } = string.Empty;
        public DateTime DateOfBirth { get; set; }
        public string Gender { get; set; } = string.Empty;
        public string? ContactNumber { get; set; }
        public string? Address { get; set; }
        public string? BloodGroup { get; set; }
    }
}
