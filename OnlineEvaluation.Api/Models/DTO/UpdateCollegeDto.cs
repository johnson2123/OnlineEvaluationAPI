namespace OnlineEvaluation.Api.Models.DTO
{
    public class UpdateCollegeDto
    {
        public string? UniversityCode { get; set; } 
        public string? Code { get; set; }
        public string? Name { get; set; }
        public string? DisplayName { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? Country { get; set; }
        public string? PostalCode { get; set; }
        public string? ContactEmail { get; set; }
        public string? ContactPhone { get; set; }
        public string? WebsiteUrl { get; set; }
        public string? Status { get; set; }
    }
}
