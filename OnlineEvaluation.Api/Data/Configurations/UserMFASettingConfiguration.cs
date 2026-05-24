using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OnlineEvaluation.Api.Models.Entities;

namespace OnlineEvaluation.Api.Data.Configurations
{
    public class UserMFASettingConfiguration : IEntityTypeConfiguration<UserMFASetting>
    {
        public void Configure(EntityTypeBuilder<UserMFASetting> builder)
        {
            builder.HasKey(m => m.Id);

            builder.ToTable("UserMFASettings");

            builder.Property(m => m.MFAType)
                .IsRequired()
                .HasMaxLength(50)
                .HasDefaultValue("None");

            builder.Property(m => m.IsMFAEnabled)
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(m => m.SecretKey)
                .HasMaxLength(256)
                .IsRequired(false);

            builder.Property(m => m.BackupCodes)
                .IsRequired(false);

            builder.Property(m => m.QRCodePath)
                .HasMaxLength(500)
                .IsRequired(false);

            builder.Property(m => m.LastUsedDate)
                .IsRequired(false);

            builder.HasOne(m => m.ApplicationUser)
                .WithMany() 
                .HasForeignKey(m => m.ApplicationUserId)
                .OnDelete(DeleteBehavior.Cascade) 
                .IsRequired();
        }
    }
}
