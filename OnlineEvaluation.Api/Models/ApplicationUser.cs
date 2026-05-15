using Microsoft.AspNetCore.Identity;

namespace OnlineEvaluation.Api.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public bool IsActive { get; set; } = true;
        public bool MustChangePassword { get; set; } = true;
        public string FullName => $"{FirstName} {LastName}";
    }
}
