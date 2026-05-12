using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OnlineEvaluation.Api.Models.Entities;

namespace OnlineEvaluation.Api.Data.Configurations
{
    public class StudyProgramConfiguration : IEntityTypeConfiguration<StudyProgram>
    {
        public void Configure(EntityTypeBuilder<StudyProgram> builder)
        {
            builder.ToTable("StudyPrograms");
            builder.HasKey(e => e.Id);
            builder.Property(e => e.Level)
                .HasConversion<string>();

            // Global Soft Delete Filter
            builder.HasQueryFilter(p => !p.IsDeleted);
        }
    }
}
