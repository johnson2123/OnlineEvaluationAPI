using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OnlineEvaluation.Api.Models.Entities;

namespace OnlineEvaluation.Api.Data.Configurations
{
    public class StaffProfileConfiguration : IEntityTypeConfiguration<StaffProfile>
    {
        public void Configure(EntityTypeBuilder<StaffProfile> builder)
        {
            builder.ToTable("StaffProfiles");

            builder.HasKey(s => s.Id);

            builder.Property(s => s.StaffGuid)
                .IsRequired()
                .HasDefaultValueSql("NEWID()"); 

            builder.HasOne(s => s.ApplicationUser)
                .WithOne()
                .HasForeignKey<StaffProfile>(s => s.ApplicationUserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(s => s.CollegeDepartment)
                .WithMany()
                .HasForeignKey(s => s.CollegeDepartmentId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Property(s => s.CollegeDepartmentAliasCode)
                .IsRequired()
                .HasMaxLength(15);

            builder.HasOne(s => s.ReportsToProfile)
                .WithMany()
                .HasForeignKey(s => s.ReportsToProfileId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Property(s => s.EmployeeId).IsRequired().HasMaxLength(50);
            builder.Property(s => s.Role).IsRequired().HasMaxLength(50);
            builder.Property(s => s.Designation).IsRequired().HasMaxLength(100);
            builder.Property(s => s.Gender).IsRequired().HasMaxLength(20);
            builder.Property(s => s.PhoneNumber).HasMaxLength(20);
            builder.Property(s => s.HighestQualification).IsRequired().HasMaxLength(100);
            builder.Property(s => s.Address).IsRequired().HasMaxLength(500);

            builder.Property(s => s.CreatedBy).IsRequired().HasMaxLength(100);


            builder.HasIndex(s => s.StaffGuid).IsUnique(); // Vital for route lookup routing!
            builder.HasIndex(s => s.EmployeeId).IsUnique();
            builder.HasIndex(s => s.CollegeDepartmentAliasCode);
        }
    }
}
