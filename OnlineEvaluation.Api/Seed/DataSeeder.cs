using Microsoft.AspNetCore.Identity;
using OnlineEvaluation.Api.Models;

namespace OnlineEvaluation.Api.Seed
{
    public class DataSeeder
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _config;

        public DataSeeder(RoleManager<IdentityRole> roleManager,
                          UserManager<ApplicationUser> userManager,
                          IConfiguration config)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _config = config;
        }

        public async Task SeedRolesAndAdminAsync()
        {
            var roles = new[] { "Admin", "Controller", "Moderator", "Faculty", "Student", "User" };
            foreach(var role in roles)
            {
                if(!await _roleManager.RoleExistsAsync(role))
                {
                    await _roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            var adminEmail = _config["Seed:AdminEmail"];
            var adminPassword = _config["Seed:AdminPassword"];
            if(string.IsNullOrWhiteSpace(adminEmail) || string.IsNullOrWhiteSpace(adminPassword)){
                return;
            }

            var admin = await _userManager.FindByEmailAsync(adminEmail);
            if(admin == null)
            {
                admin = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    FirstName = "Admin",
                    LastName = "System",
                    EmailConfirmed = true,
                };

                var result = await _userManager.CreateAsync(admin, adminPassword);
                if(result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(admin, "Admin");
                }
            }
        }
    }
}
