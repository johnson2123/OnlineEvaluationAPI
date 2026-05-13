namespace OnlineEvaluation.Api.Models.DTO
{
    public class BranchDto
    {
        public int Id { get; set; }
        public Guid Guid { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? DisplayName { get; set; }
        public string? Description { get; set; }
        public bool IsActive { get; set; }
    }
}
