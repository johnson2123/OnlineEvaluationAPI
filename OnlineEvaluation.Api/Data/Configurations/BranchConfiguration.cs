using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OnlineEvaluation.Api.Models.Entities;

namespace OnlineEvaluation.Api.Data.Configurations
{
    public class BranchConfiguration : IEntityTypeConfiguration<Branch>
    {
        public void Configure(EntityTypeBuilder<Branch> builder)
        {
            builder.ToTable("Branches");

            builder.HasKey(b => b.Id);

            builder.HasIndex(b => b.Guid).IsUnique();
            builder.HasIndex(b => b.Code).IsUnique();

            // Property Constraints
            builder.Property(b => b.Code)
                .IsRequired()
                .HasMaxLength(20);

            builder.Property(b => b.Name)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(b => b.DisplayName)
                .HasMaxLength(250);

            builder.Property(b => b.CreatedBy)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(b => b.UpdatedBy)
                .HasMaxLength(100);

            builder.Property(b => b.DeletedBy)
                .HasMaxLength(100);


            builder.HasQueryFilter(b => !b.IsDeleted);
        }
    }
}
