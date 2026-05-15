using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OnlineEvaluation.Api.Models;

namespace OnlineEvaluation.Api.Data.Configurations
{
    public class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
    {
        public void Configure(EntityTypeBuilder<ApplicationUser> builder)
        {
            builder.Property(u => u.FirstName)
                    .IsRequired()
                    .HasMaxLength(100);

            builder.Property(u => u.LastName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(u => u.IsActive)
                .HasDefaultValue(true);

            builder.Property(u => u.MustChangePassword)
                .IsRequired()
                .HasDefaultValue(true);

            builder.HasIndex(u => u.FirstName);
            builder.HasIndex(u => u.LastName);
        }
    }
}
