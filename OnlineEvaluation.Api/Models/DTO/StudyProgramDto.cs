namespace OnlineEvaluation.Api.Models.DTO
{
    public class StudyProgramDto
    {
        public int Id { get; set; }
        public Guid Guid { get; set; }
        public string Name { get; set; } = string.Empty;
        public string ShortName { get; set; } = string.Empty;
        public string Level { get; set; } = string.Empty; // String for React
        public int DurationInYears { get; set; }
        public int TotalSemesters { get; set; }
        public bool IsActive { get; set; }
    }
}
