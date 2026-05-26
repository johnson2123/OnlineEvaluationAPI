using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OnlineEvaluation.Api.Models.Entities;

namespace OnlineEvaluation.Api.Data.Configurations
{
    public class OtpLogConfiguration : IEntityTypeConfiguration<OtpLog>
    {
        public void Configure(EntityTypeBuilder<OtpLog> builder)
        {
            builder.ToTable("OtpLogs");

            builder.HasKey(o => o.Id);
            builder.Property(o => o.Id).UseIdentityColumn(); // INT IDENTITY(1,1)

            builder.Property(o => o.Guid)
                   .IsRequired()
                   .HasDefaultValueSql("NEWID()");

            builder.HasIndex(o => o.Guid).IsUnique();

            builder.Property(o => o.ApplicationUserId).IsRequired();
            builder.Property(o => o.OtpCode).IsRequired().HasMaxLength(10);
            builder.Property(o => o.OtpType).IsRequired().HasMaxLength(30);
            builder.Property(o => o.SentTo).IsRequired().HasMaxLength(150);
            builder.Property(o => o.ExpiryTime).IsRequired();
            builder.Property(o => o.IsUsed).IsRequired().HasDefaultValue(false);
            builder.Property(o => o.AttemptCount).IsRequired().HasDefaultValue(0);
            builder.Property(o => o.IPAddress).HasMaxLength(50);
            builder.Property(o => o.DeviceInfo).HasMaxLength(250);
            builder.Property(o => o.CreatedDate).IsRequired().HasDefaultValueSql("GETDATE()");

            builder.HasOne(o => o.ApplicationUser)
                   .WithMany()
                   .HasForeignKey(o => o.ApplicationUserId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(o => new { o.ApplicationUserId, o.IsUsed, o.ExpiryTime });
        }
    }
}
