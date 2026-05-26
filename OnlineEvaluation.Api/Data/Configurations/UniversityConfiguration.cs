using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OnlineEvaluation.Api.Models.Entities;

namespace OnlineEvaluation.Api.Data.Configurations
{
    public class UniversityConfiguration : IEntityTypeConfiguration<University>
    {
        public void Configure(EntityTypeBuilder<University> builder)
        {
            builder.ToTable("Universities");

            builder.HasKey(u => u.Id);

            builder.Property(u => u.Guid).IsRequired();
            builder.Property(u => u.Code).IsRequired().HasMaxLength(50);
            builder.Property(u => u.Name).IsRequired().HasMaxLength(250);
            builder.Property(u => u.DisplayName).HasMaxLength(250);
            builder.Property(u => u.Address).HasMaxLength(500);
            builder.Property(u => u.City).HasMaxLength(150);
            builder.Property(u => u.State).HasMaxLength(150);
            builder.Property(u => u.Country).HasMaxLength(3);
            builder.Property(u => u.PostalCode).HasMaxLength(30);
            builder.Property(u => u.ContactEmail).HasMaxLength(254);
            builder.Property(u => u.ContactPhone).HasMaxLength(30);
            builder.Property(u => u.WebsiteUrl).HasMaxLength(500);
            builder.Property(u => u.AccreditationBody).HasMaxLength(200);
            builder.Property(u => u.Status).IsRequired().HasMaxLength(50);

            builder.Property(u => u.SubscriptionPlan).HasMaxLength(100);
            builder.Property(u => u.SubscriptionStatus).HasMaxLength(50);
            builder.Property(u => u.PlanAmount).HasColumnType("decimal(18,2)");
            builder.Property(u => u.BillingCycle).HasMaxLength(50);
            builder.Property(u => u.SubscriptionStartDate);
            builder.Property(u => u.SubscriptionEndDate);

            builder.Property(u => u.IsDeleted).HasDefaultValue(false);
            builder.Property(u => u.CreatedAt).IsRequired();
            builder.Property(u => u.CreatedByUserId).HasMaxLength(450);
            builder.Property(u => u.UpdatedByUserId).HasMaxLength(450);
            builder.Property(u => u.DeletedByUserId).HasMaxLength(450);
            builder.Property(u => u.RowVersion).IsRowVersion();


            builder.HasIndex(u => u.Guid)
                   .IsUnique()
                   .HasFilter("[IsDeleted] = 0");
            builder.HasIndex(u => u.Code)
                   .IsUnique()
                   .HasFilter("[IsDeleted] = 0");
            builder.HasIndex(u => u.Name);
            builder.HasIndex(u => new { u.IsDeleted, u.Status });

            builder.HasQueryFilter(u => !u.IsDeleted);
        }
    }
}
