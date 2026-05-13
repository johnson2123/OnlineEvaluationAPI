namespace OnlineEvaluation.Api.Models.DTO
{
    public class UpdateAcademicMapDto
    {
        public int Id { get; set; }
        public Guid Guid { get; set; }
        public int CollegeId { get; set; }
        public int StudyProgramId { get; set; }
        public int BranchId { get; set; }
        public string? AliasCode { get; set; }
        public bool IsActive { get; set; }
    }
}
