using System.ComponentModel.DataAnnotations;

namespace OnlineEvaluation.Api.Models.Entities
{
    public class University
    {

        public int Id { get; set; }
        public Guid Guid { get; set; } = Guid.NewGuid();
        public string Code { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string? DisplayName { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? Country { get; set; }
        public string? PostalCode { get; set; }
        public string? ContactEmail { get; set; }
        public string? ContactPhone { get; set; }
        public string? WebsiteUrl { get; set; }
        public string? AccreditationBody { get; set; }
        public string Status { get; set; } = "Active";

        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string? CreatedByUserId { get; set; }

        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedByUserId { get; set; }

        public bool IsDeleted { get; set; } = false;
        public DateTime? DeletedAt { get; set; }
        public string? DeletedByUserId { get; set; }

        [Timestamp]
        public byte[]? RowVersion { get; set; }


    }
}
