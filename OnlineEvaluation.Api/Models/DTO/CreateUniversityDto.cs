using System.ComponentModel.DataAnnotations;

namespace OnlineEvaluation.Api.Models.DTO
{
    public class CreateUniversityDto
    {
        [Required] 
        public string Code { get; set; } = null!;

        [Required]
        public string Name { get; set; } = null!;

        public string? DisplayName { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? Country { get; set; }
        public string? PostalCode { get; set; }

        [EmailAddress]
        public string? ContactEmail { get; set; }
        public string? ContactPhone { get; set; }

        [Url]
        public string? WebsiteUrl { get; set; }
        public string? AccreditationBody { get; set; }
        public string? Status { get; set; } = "Active";
    }
}
