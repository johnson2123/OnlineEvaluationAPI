namespace OnlineEvaluation.Api.Models.DTO
{
    public class AcademicMapInitDto
    {
        public List<LookUpDto> Colleges { get; set; } = new();
        public List<LookUpDto> StudyPrograms { get; set; } = new();
        public List<LookUpDto> Branches { get; set; } = new();

    }
}
