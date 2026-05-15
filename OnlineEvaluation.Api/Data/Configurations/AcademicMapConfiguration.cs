using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OnlineEvaluation.Api.Models.Entities;

namespace OnlineEvaluation.Api.Data.Configurations
{
    public class AcademicMapConfiguration : IEntityTypeConfiguration<AcademicMap>
    {
        public void Configure(EntityTypeBuilder<AcademicMap> builder)
        {
            builder.ToTable("AcademicMaps");

            builder.HasKey(x => x.Id);


            builder.HasIndex(x => x.Guid).IsUnique();

            // --- THE UNIQUE TRIPLE CONSTRAINT ---
            // This prevents creating "AUCE + BTECH + CSE" more than once.
            builder.HasIndex(x => new { x.CollegeId, x.StudyProgramId, x.BranchId, x.Regulation })
                .IsUnique()
                .HasDatabaseName("IX_Unique_Academic_Path_Regulation");

            builder.Property(x => x.Regulation)
                .IsRequired()
                .HasMaxLength(20);

            builder.Property(x => x.AliasCode)
                .HasMaxLength(100);

            // --- Relationships ---

            builder.HasOne(x => x.College)
                .WithMany() // Or WithMany(c => c.AcademicMaps) if you add that collection to College
                .HasForeignKey(x => x.CollegeId)
                .OnDelete(DeleteBehavior.Restrict); // Prevent deleting a College if maps exist

            builder.HasOne(x => x.StudyProgram)
                .WithMany()
                .HasForeignKey(x => x.StudyProgramId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Branch)
                .WithMany()
                .HasForeignKey(x => x.BranchId)
                .OnDelete(DeleteBehavior.Restrict);


            builder.HasQueryFilter(x => !x.IsDeleted);
        }
    }
}
