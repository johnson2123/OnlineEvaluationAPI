namespace OnlineEvaluation.Api.Models.DTO
{
    public class CollegeDepartmentDto
    {
        public int Id { get; set; }
        public Guid Guid { get; set; }
        public string AliasCode { get; set; } = string.Empty;

        public int CollegeId { get; set; }
        public string CollegeName { get; set; } = string.Empty;

        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; } = string.Empty;
        public string DepartmentCode { get; set; } = string.Empty;

        public bool IsActive { get; set; }
    }
}
