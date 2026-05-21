using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OnlineEvaluation.Api.Models.Entities;

namespace OnlineEvaluation.Api.Data.Configurations
{
    public class DepartmentConfiguration : IEntityTypeConfiguration<Department>
    {
        public void Configure(EntityTypeBuilder<Department> builder)
        {
            builder.ToTable("Departments");

            builder.HasKey(d => d.Id);

            builder.HasIndex(d => d.Guid).IsUnique();
            builder.HasIndex(d => d.Code).IsUnique();

            builder.Property(d => d.Code)
                .IsRequired()
                .HasMaxLength(20);

            builder.Property(d => d.Name)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(d => d.DisplayName)
                .HasMaxLength(250);

            builder.Property(d => d.Description)
                .HasMaxLength(500);

            builder.Property(d => d.CreatedBy)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(d => d.UpdatedBy)
                .HasMaxLength(100);

            builder.Property(d => d.DeletedBy)
                .HasMaxLength(100);

            builder.HasQueryFilter(d => !d.IsDeleted);
        }
    }
}
