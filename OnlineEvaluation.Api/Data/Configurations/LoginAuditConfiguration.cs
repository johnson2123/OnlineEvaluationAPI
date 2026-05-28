using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OnlineEvaluation.Api.Models.Entities;

namespace OnlineEvaluation.Api.Data.Configurations
{
    public class LoginAuditConfiguration : IEntityTypeConfiguration<LoginAudit>
    {
        public void Configure(EntityTypeBuilder<LoginAudit> builder)
        {
            builder.ToTable("LoginAudits");

            builder.HasKey(la => la.AuditId);

            builder.Property(la => la.AuditId)
                .ValueGeneratedOnAdd();

            builder.Property(la => la.LoginTime)
                .IsRequired()
                .HasDefaultValueSql("GETUTCDATE()");

            builder.Property(la => la.CreatedDate)
                .IsRequired()
                .HasDefaultValueSql("GETUTCDATE()");

            builder.Property(la => la.IPAddress)
                .HasMaxLength(50);

            builder.Property(la => la.DeviceInfo)
                .HasMaxLength(250);

            builder.Property(la => la.BrowserInfo)
                .HasMaxLength(250);

            builder.Property(la => la.OperatingSystem)
                .HasMaxLength(100);

            builder.Property(la => la.LoginStatus)
                .IsRequired()
                .HasMaxLength(20);

            builder.Property(la => la.FailureReason)
                .HasMaxLength(500);

            builder.Property(la => la.SessionId)
                .HasMaxLength(200);

            builder.Property(la => la.LoginLocation)
                .HasMaxLength(150);

            builder.HasOne(la => la.User)
                .WithMany() // Assuming ApplicationUser doesn't hold an inverse collection of 'LoginAudits'
                .HasForeignKey(la => la.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
