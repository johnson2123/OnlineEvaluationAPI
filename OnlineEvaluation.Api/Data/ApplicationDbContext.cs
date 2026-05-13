using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using OnlineEvaluation.Api.Data.Configurations;
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
        public DbSet<University> Universities { get; set; }
        public DbSet<College> Colleges { get; set; }
        public DbSet<StudyProgram> StudyPrograms { get; set; }
        public DbSet<Branch> Branches { get; set; }
        public DbSet<Subject> Subjects { get; set; }

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

            builder.ApplyConfiguration(new UniversityConfiguration());
            builder.ApplyConfiguration(new CollegeConfiguration());
            builder.ApplyConfiguration(new StudyProgramConfiguration());
            builder.ApplyConfiguration(new BranchConfiguration());
            builder.ApplyConfiguration(new SubjectConfiguration());
        }

    }
}
