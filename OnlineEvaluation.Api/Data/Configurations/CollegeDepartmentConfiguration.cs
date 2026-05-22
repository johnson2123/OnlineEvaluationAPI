using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OnlineEvaluation.Api.Models.Entities;

namespace OnlineEvaluation.Api.Data.Configurations
{
    public class CollegeDepartmentConfiguration : IEntityTypeConfiguration<CollegeDepartment>
    {
        public void Configure(EntityTypeBuilder<CollegeDepartment> builder)
        {
            builder.ToTable("CollegeDepartments");

            builder.HasKey(cd => cd.Id);

            builder.Property(cd => cd.Guid).IsRequired();
            builder.HasIndex(cd => cd.Guid).IsUnique();

            builder.Property(cd => cd.AliasCode)
                   .HasMaxLength(100)
                   .IsRequired();

            builder.HasIndex(cd => cd.AliasCode)
                   .IsUnique()
                   .HasFilter("[IsDeleted] = 0"); 

            builder.HasIndex(cd => new { cd.CollegeId, cd.DepartmentId })
                   .IsUnique()
                   .HasFilter("[IsDeleted] = 0");

            builder.Property(cd => cd.CreatedBy).IsRequired().HasMaxLength(150);
            builder.Property(cd => cd.UpdatedBy).HasMaxLength(150);
            builder.Property(cd => cd.DeletedBy).HasMaxLength(150);

            builder.HasOne(cd => cd.College)
                   .WithMany() 
                   .HasForeignKey(cd => cd.CollegeId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(cd => cd.Department)
                   .WithMany() 
                   .HasForeignKey(cd => cd.DepartmentId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasQueryFilter(cd => !cd.IsDeleted);
        }
    }
}
