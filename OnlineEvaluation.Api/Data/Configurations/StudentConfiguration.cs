using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OnlineEvaluation.Api.Models.Entities;

namespace OnlineEvaluation.Api.Data.Configurations
{
    public class StudentConfiguration : IEntityTypeConfiguration<Student>
    {
        public void Configure(EntityTypeBuilder<Student> builder)
        {
            builder.ToTable("Students");
            builder.HasKey(s => s.Id);
            builder.Property(s => s.Guid)
                .IsRequired();
            builder.HasIndex(s => s.Guid)
                .IsUnique()
                .HasFilter("[IsDeleted] = 0");

            builder.Property(s => s.RegistrationNumber).IsRequired().HasMaxLength(30);
            builder.Property(s => s.Batch).IsRequired().HasMaxLength(15);
            builder.Property(s => s.AcademicAliasCode).IsRequired().HasMaxLength(100);
            builder.Property(s => s.FatherName).IsRequired().HasMaxLength(100);
            builder.Property(s => s.Gender).IsRequired().HasMaxLength(15);
            builder.Property(s => s.ContactNumber).HasMaxLength(20);
            builder.Property(s => s.Address).HasMaxLength(500);
            builder.Property(s => s.BloodGroup).HasMaxLength(10);
            builder.Property(s => s.CreatedBy).IsRequired().HasMaxLength(100);
            builder.Property(s => s.UpdatedBy).HasMaxLength(100);
            builder.Property(s => s.DeletedBy).HasMaxLength(100);

            builder.HasOne(s => s.User)
                .WithMany()
                .HasForeignKey(s => s.ApplicationUserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(s => s.AcademicMap)
                .WithMany(am => am.Students)
                .HasForeignKey(s => s.AcademicMapId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(s => s.RegistrationNumber)
                    .IsUnique()
                    .HasFilter("[IsDeleted] = 0");
            builder.HasIndex(s => s.ApplicationUserId).IsUnique();
            builder.HasIndex(s => s.IsActive);

            builder.HasQueryFilter(s => !s.IsDeleted);
        }
    }
}
