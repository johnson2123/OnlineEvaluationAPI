using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OnlineEvaluation.Api.Models.Entities;

namespace OnlineEvaluation.Api.Data.Configurations
{
    public class StudentAcademicRecordConfiguration : IEntityTypeConfiguration<StudentAcademicRecord>
    {
        public void Configure(EntityTypeBuilder<StudentAcademicRecord> builder)
        {
            builder.ToTable("StudentAcademicRecords");
            builder.HasKey(r => r.Id);

            builder.Property(r => r.Guid).IsRequired();
            builder.Property(r => r.AcademicAliasCode).IsRequired().HasMaxLength(100);
            builder.Property(r => r.AcademicYear).IsRequired().HasMaxLength(15);
            builder.Property(r => r.AcademicSessionSlug).IsRequired().HasMaxLength(100);
            builder.Property(r => r.CreatedBy).IsRequired().HasMaxLength(100);
            builder.Property(r => r.UpdatedBy).HasMaxLength(100);

            builder.HasQueryFilter(r => !r.AcademicMap.IsDeleted && !r.Student.IsDeleted);

            builder.Property(r => r.Standing)
                .IsRequired()
                .HasConversion<int>();

            // Link historical timeline back to main profile 
            builder.HasOne(r => r.Student)
                .WithMany(s => s.StudentAcademicRecords) 
                .HasForeignKey(r => r.StudentId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(r => r.AcademicMap)
                .WithMany()
                .HasForeignKey(r => r.AcademicMapId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(r => r.AcademicSessionSlug);

            builder.HasIndex(r => new { r.StudentId, r.Semester })
                .IsUnique();

            // INDEX 3: Rapid targeting of active student scopes across dashboards
            builder.HasIndex(r => new { r.IsCurrentSemester, r.Standing });
        }
    }
}
