using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OnlineEvaluation.Api.Models.Entities;

namespace OnlineEvaluation.Api.Data.Configurations
{
    public class CollegeConfiguration : IEntityTypeConfiguration<College>
    {
        public void Configure(EntityTypeBuilder<College> builder)
        {

            builder.HasKey(c => c.Id);

            builder.Property(c => c.Code).IsRequired().HasMaxLength(50);
            builder.Property(c => c.Name).IsRequired().HasMaxLength(250);
            builder.Property(c => c.UniversityCode).IsRequired().HasMaxLength(50);
            builder.Property(c => c.Status).HasMaxLength(50).HasDefaultValue("Active");

            // Relationship
            builder.HasOne(c => c.University)
                   .WithMany() 
                   .HasPrincipalKey(u => u.Code) 
                   .HasForeignKey(c => c.UniversityCode) 
                   .OnDelete(DeleteBehavior.Restrict); // Prevent University deletion if Colleges exist

            
            builder.HasIndex(c => new { c.UniversityCode, c.Code })
                   .IsUnique()
                   .HasFilter("[IsDeleted] = 0");

            // Global Query Filter for Soft Delete
            builder.HasQueryFilter(c => !c.IsDeleted);


            builder.Property(c => c.RowVersion).IsRowVersion();
        }
    }
}
