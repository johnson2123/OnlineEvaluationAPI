using OnlineEvaluation.Api.Constants;

namespace OnlineEvaluation.Api.Models.Entities
{
    public class StudentAcademicRecord
    {
        public int Id { get; set; }
        public Guid Guid { get; set; } = Guid.NewGuid();

        public int StudentId { get; set; }
        public virtual Student Student { get; set; }

        public int AcademicMapId { get; set; }
        public virtual AcademicMap AcademicMap { get; set; }
        public string AcademicAliasCode { get; set; } = string.Empty;

        public int Semester { get; set; } // e.g., 1, 2, 3
        public string AcademicYear { get; set; } = string.Empty; 
        public string AcademicSessionSlug { get; set; } = string.Empty;

        // --- NEW LIFECYCLE AND PROMOTION ENGINE FIELDS ---
        /// Tracks the current standing for this specific term (InProgress, Promoted, Detained, Withdrawn)

        public SemesterStanding Standing { get; set; } = SemesterStanding.Active;
        public bool IsCurrentSemester { get; set; }


        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string CreatedBy { get; set; } = string.Empty;
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }
    }
}
