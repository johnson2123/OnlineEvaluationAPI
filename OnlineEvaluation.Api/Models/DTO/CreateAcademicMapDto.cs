namespace OnlineEvaluation.Api.Models.DTO
{
    public class CreateAcademicMapDto
    {
        public int CollegeId { get; set; }
        public int StudyProgramId { get; set; }
        public int BranchId { get; set; }
        public string? AliasCode { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
