using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OnlineEvaluation.Api.Models.Entities;

namespace OnlineEvaluation.Api.Data.Configurations
{
    public class AccountLockConfiguration : IEntityTypeConfiguration<AccountLock>
    {
        public void Configure(EntityTypeBuilder<AccountLock> builder)
        {
            builder.ToTable("AccountLocks");

            builder.HasKey(al => al.SecurityId);

            builder.Property(al => al.SecurityId)
                .ValueGeneratedOnAdd();

            builder.Property(al => al.FailedLoginAttempts)
                .IsRequired()
                .HasDefaultValue(0);

            builder.Property(al => al.IsAccountLocked)
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(al => al.SecurityQuestionEnabled)
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(al => al.CreatedDate)
                .IsRequired()
                .HasDefaultValueSql("GETUTCDATE()"); 

            builder.HasOne(al => al.User)
                .WithOne() // Assuming ApplicationUser doesn't hold a inverse 'AccountLock' navigation property
                .HasForeignKey<AccountLock>(al => al.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
