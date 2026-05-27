using AutoMapper;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using OnlineEvaluation.Api.Data;
using OnlineEvaluation.Api.Mappings;
using OnlineEvaluation.Api.Models;
using OnlineEvaluation.Api.Seed;
using OnlineEvaluation.Api.Services;
using OnlineEvaluation.Api.Services.Background;
using OnlineEvaluation.Api.Services.IServices;
using OnlineEvaluation.Api.Settings;
using OnlineEvaluation.Api.Validators;
using System.Security.Claims;
using System.Text;

namespace OnlineEvaluation.Api.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration config)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(config.GetConnectionString("DefaultConnection")));

            services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.User.RequireUniqueEmail = true;
                options.Password.RequiredLength = 8;
                options.Lockout.MaxFailedAccessAttempts = 10;
                options.SignIn.RequireConfirmedEmail = false; //for now
            })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

            //services.AddAutoMapper(cfg => { }, AppDomain.CurrentDomain.GetAssemblies());
            services.AddAutoMapper(cfg =>
            {
                
            }, new[] { typeof(UniversityProfile).Assembly });
            services.AddValidatorsFromAssemblyContaining<CreateUniversityValidator>();
            services.AddFluentValidationAutoValidation();
            services.AddFluentValidationClientsideAdapters();


            var key = Encoding.UTF8.GetBytes(config["Jwt:Key"] ?? "MyFallbackSecreKeyHere_ThisIsSoSoLongAndTough@!#$");
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = true;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = config["Jwt:Issuer"],
                    ValidAudience = config["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(key),

                    RoleClaimType = ClaimTypes.Role,
                    NameClaimType = ClaimTypes.NameIdentifier
                };
            });

            services.Configure<EmailSettings>(config.GetSection("EmailSettings"));
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<DataSeeder>();
            services.AddScoped<IUniversityService, UniversityService>();
            services.AddScoped<ICollegeService, CollegeService>();
            services.AddScoped<IStudyProgramService, StudyProgramService>();
            services.AddScoped<IBranchService, BranchService>();
            services.AddScoped<ISubjectService, SubjectService>();
            services.AddScoped<IAcademicMapService, AcademicMapService>();
            services.AddScoped<IStudentOnboardingService, StudentOnboardingService>();
            services.AddScoped<IExamCodeSpecificationService, ExamCodeSpecificationService>();
            services.AddScoped<IStudentProfileService, StudentProfileService>();
            services.AddScoped<IStaffProfileService, StaffProfileService>();
            services.AddScoped<IDepartmentService, DepartmentService>();
            services.AddScoped<ICollegeDepartmentService, CollegeDepartmentService>();
            services.AddScoped<IStaffOnboardingService, StaffOnboardingService>();
            services.AddScoped<IMfaSecurityService, MfaSecurityService>();
            services.AddScoped<IOtpService, OtpService>();
            services.AddSingleton<IEmailQueue, BackgroundEmailQueue>();
            services.AddHostedService<EmailWorkerService>();



            return services;
        }
    }
}
