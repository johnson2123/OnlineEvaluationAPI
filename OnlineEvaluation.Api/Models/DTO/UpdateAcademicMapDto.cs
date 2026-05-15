namespace OnlineEvaluation.Api.Models.DTO
{
    public class UpdateAcademicMapDto
    {
        public int CollegeId { get; set; }
        public int StudyProgramId { get; set; }
        public int BranchId { get; set; }
        public string Regulation { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }
}
