namespace OnlineEvaluation.Api.Models.DTO
{
    public class StudentProfileDto
    {
        public Guid ProfileGuid { get; set; }


        public string RegistrationNumber { get; set; } = string.Empty;
        public string Batch { get; set; } = string.Empty;
        public string BranchName { get; set; } = string.Empty;
        public string Regulation { get; set; } = string.Empty;

        public string FullName { get; set; } = string.Empty;
        public string FatherName { get; set; } = string.Empty;
        public DateTime DateOfBirth { get; set; }
        public string Gender { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;


        public string? ContactNumber { get; set; }
        public string? Address { get; set; }
        public string? BloodGroup { get; set; }
    }
}
