namespace OnlineEvaluation.Api.Models.Entities
{
    public class Branch
    {
        public int Id { get; set; }

        // Secure unique identifier for frontend and API routing
        public Guid Guid { get; set; } = Guid.NewGuid();

        public string Code { get; set; } = string.Empty; // "CSE", "ECE"
        public string Name { get; set; } = string.Empty; // "Computer Science Engineering"
        public string? DisplayName { get; set; }
        public string? Description { get; set; }
        public bool IsActive { get; set; } = true;


        public bool IsDeleted { get; set; } = false;
        public DateTime? DeletedAt { get; set; }
        public string? DeletedBy { get; set; }


        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string CreatedBy { get; set; } = string.Empty; 
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }
    }
}
