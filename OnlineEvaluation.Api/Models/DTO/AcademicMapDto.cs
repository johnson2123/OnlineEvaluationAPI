namespace OnlineEvaluation.Api.Models.DTO
{
    public class AcademicMapDto
    {
        public int Id { get; set; }
        public Guid Guid { get; set; }
        public string? AliasCode { get; set; }
        public bool IsActive { get; set; }

   
        public int CollegeId { get; set; }
        public int StudyProgramId { get; set; }
        public int BranchId { get; set; }

        // Flattened properties for the UI (Very helpful for tables/dropdowns)
        public string CollegeName { get; set; } = string.Empty;
        public string ProgramName { get; set; } = string.Empty;
        public string BranchName { get; set; } = string.Empty;
        public string Regulation { get; set; } = string.Empty;
    }
}
