using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OnlineEvaluation.Api.Models.Entities;

namespace OnlineEvaluation.Api.Data.Configurations
{
    public class ExamCodeSpecificationConfiguration : IEntityTypeConfiguration<ExamCodeSpecification>
    {
        public void Configure(EntityTypeBuilder<ExamCodeSpecification> builder)
        {
            builder.ToTable("ExamCodeSpecifications");

            builder.HasKey(e => e.Id);
            builder.HasIndex(e => e.Guid).IsUnique();

            builder.HasIndex(e => e.ExamSpecCode).IsUnique();

            builder.Property(e => e.ExamSpecCode)
                   .HasMaxLength(100)
                   .IsRequired();

            builder.Property(e => e.CreatedBy)
                   .HasMaxLength(150)
                   .IsRequired();

            builder.Property(e => e.UpdatedBy).HasMaxLength(150);
            builder.Property(e => e.DeletedBy).HasMaxLength(150);

            builder.HasOne(d => d.Subject)
                   .WithMany()
                   .HasForeignKey(d => d.SubjectId)
                   .OnDelete(DeleteBehavior.Restrict);


            builder.HasIndex(e => new { e.AcademicMapId, e.Semester });

            builder.HasQueryFilter(e => !e.IsDeleted);
        }
    }
}
