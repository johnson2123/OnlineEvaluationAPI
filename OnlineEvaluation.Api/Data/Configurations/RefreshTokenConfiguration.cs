using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OnlineEvaluation.Api.Models.Entities;

namespace OnlineEvaluation.Api.Data.Configurations
{
    public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
    {
        public void Configure(EntityTypeBuilder<RefreshToken> builder)
        {
            builder.ToTable("RefreshTokens");

            builder.HasKey(t => t.Id);

            builder.Property(t => t.TokenHash)
                .IsRequired()
                .HasMaxLength(512);

            // Indexing the hash is critical for login performance
            builder.HasIndex(t => t.TokenHash);

            builder.Property(t => t.UserId)
                .IsRequired();

            builder.HasOne(t => t.User)
                .WithMany() // We don't need the collection on the User side
                .HasForeignKey(t => t.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Property(t => t.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");
        }
    }
}
