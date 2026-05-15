using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using OnlineEvaluation.Api.Data.Configurations;
using OnlineEvaluation.Api.Models;
using OnlineEvaluation.Api.Models.Entities;
using System.Reflection.Emit;

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
        public DbSet<AcademicMap> AcademicMaps { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.ApplyConfiguration(new ApplicationUserConfiguration());
            builder.ApplyConfiguration(new RefreshTokenConfiguration());
            builder.ApplyConfiguration(new UniversityConfiguration());
            builder.ApplyConfiguration(new CollegeConfiguration());
            builder.ApplyConfiguration(new StudyProgramConfiguration());
            builder.ApplyConfiguration(new BranchConfiguration());
            builder.ApplyConfiguration(new SubjectConfiguration());
            builder.ApplyConfiguration(new AcademicMapConfiguration());
        }

    }
}
