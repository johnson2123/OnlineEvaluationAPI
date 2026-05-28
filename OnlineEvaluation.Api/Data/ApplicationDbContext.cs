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
        public DbSet<Student> Students { get; set; }
        public DbSet<StudentAcademicRecord> StudentAcademicRecords { get; set; }
        public DbSet<ExamCodeSpecification> ExamCodeSpecifications { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<CollegeDepartment> CollegeDepartments { get; set; }
        public DbSet<StaffProfile> StaffProfiles { get; set; }
        public DbSet<UserMFASetting> UserMFASettings { get; set; }
        public DbSet<OtpLog> OtpLogs { get; set; }
        public DbSet<AccountLock> AccountLocks { get; set; }
        public DbSet<LoginAudit> LoginAudits { get; set; }

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
            builder.ApplyConfiguration(new StudentConfiguration());
            builder.ApplyConfiguration(new StudentAcademicRecordConfiguration());
            builder.ApplyConfiguration(new ExamCodeSpecificationConfiguration());
            builder.ApplyConfiguration(new DepartmentConfiguration());
            builder.ApplyConfiguration(new CollegeDepartmentConfiguration());
            builder.ApplyConfiguration(new StaffProfileConfiguration());
            builder.ApplyConfiguration(new UserMFASettingConfiguration());
            builder.ApplyConfiguration(new OtpLogConfiguration());
            builder.ApplyConfiguration(new AccountLockConfiguration());
            builder.ApplyConfiguration(new LoginAuditConfiguration());
        }
    }

    
}
