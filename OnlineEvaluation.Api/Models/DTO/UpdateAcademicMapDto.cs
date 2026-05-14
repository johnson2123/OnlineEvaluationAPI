namespace OnlineEvaluation.Api.Models.DTO
{
    public class UpdateAcademicMapDto
    {
        public int CollegeId { get; set; }
        public int StudyProgramId { get; set; }
        public int BranchId { get; set; }
        public bool IsActive { get; set; }
    }
}
