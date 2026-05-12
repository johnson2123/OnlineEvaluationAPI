namespace OnlineEvaluation.Api.Models.DTO
{
    public class ProgramDto
    {
        public int Id { get; set; }
        public Guid Guid { get; set; }
        public string Name { get; set; } = string.Empty;
        public string ShortName { get; set; } = string.Empty;
        public string Level { get; set; } = string.Empty; // We return string for easy display
        public int DurationInYears { get; set; }
        public int TotalSemesters { get; set; }
        public bool IsActive { get; set; }
    }
}
