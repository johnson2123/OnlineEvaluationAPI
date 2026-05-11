using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using OnlineEvaluation.Api.Models;
using OnlineEvaluation.Api.Models.Entities;

namespace OnlineEvaluation.Api.Data
{
    public class ApplicationDbContext :IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
            
        }

        public DbSet<RefreshToken> RefreshTokens { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<RefreshToken>(entity =>
            {
                entity.HasKey(u => u.Id);
                entity.HasIndex(u => u.TokenHash).IsUnique();
                entity.HasOne(u => u.User)
                      .WithMany()
                      .HasForeignKey(u => u.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
            });
        }

    }
}
