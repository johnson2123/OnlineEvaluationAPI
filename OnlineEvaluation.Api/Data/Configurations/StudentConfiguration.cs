using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OnlineEvaluation.Api.Models.Entities;

namespace OnlineEvaluation.Api.Data.Configurations
{
    public class StudentConfiguration : IEntityTypeConfiguration<Student>
    {
        public void Configure(EntityTypeBuilder<Student> builder)
        {
            builder.HasKey(s => s.Id);

            builder.HasIndex(s => s.RegistrationNumber)
                   .IsUnique();

            builder.HasOne(s => s.User)
                   .WithOne() // A user is linked to one student profile
                   .HasForeignKey<Student>(s => s.ApplicationUserId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(s => s.AcademicMap)
                   .WithMany()
                   .HasForeignKey(s => s.AcademicMapId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.Property(s => s.RegistrationNumber)
                   .IsRequired()
                   .HasMaxLength(50);

            builder.Property(s => s.Batch)
                   .IsRequired()
                   .HasMaxLength(20);

            builder.Property(s => s.AcademicAliasCode)
                   .IsRequired()
                   .HasMaxLength(20);

            builder.Property(s => s.CreatedBy)
                   .IsRequired();


            builder.HasQueryFilter(s => !s.IsDeleted);
        }
    }
}
