using FluentValidation;
using OnlineEvaluation.Api.Models.DTO;

namespace OnlineEvaluation.Api.Validators
{
    public class StaffRegistrationValidator : AbstractValidator<StaffRegistrationDto>
    {
        private readonly string[] _allowedGenders = { "Male", "Female", "Other" };
        private readonly string[] _allowedRoles = { "Controller", "Moderator", "Faculty" };
        public StaffRegistrationValidator()
        {

            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("First name is required.")
                .MaximumLength(50).WithMessage("First name cannot exceed 50 characters.");

            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("Last name is required.")
                .MaximumLength(50).WithMessage("Last name cannot exceed 50 characters.");

            RuleFor(x => x.EmployeeId)
                .NotEmpty().WithMessage("Employee ID is required.")
                .MaximumLength(20).WithMessage("Employee ID cannot exceed 20 characters.");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email address is required.")
                .EmailAddress().WithMessage("A valid email address is required.");

            RuleFor(x => x.CollegeDepartmentId)
                .GreaterThan(0).WithMessage("A valid College Department ID must be specified.");

            RuleFor(x => x.Role)
                .NotEmpty().WithMessage("System authorization role is required.")
                .Must(role => _allowedRoles.Contains(role?.Trim(), StringComparer.OrdinalIgnoreCase))
                .WithMessage($"Invalid role specification. Allowed options are: {string.Join(", ", _allowedRoles)}.");

            RuleFor(x => x.Designation)
                .NotEmpty().WithMessage("Official designation is required.");

            RuleFor(x => x.Gender)
                .NotEmpty().WithMessage("Gender is required.")
                .Must(gender => _allowedGenders.Contains(gender?.Trim(), StringComparer.OrdinalIgnoreCase))
                .WithMessage($"Gender must be either: {string.Join(", ", _allowedGenders)}.");

            RuleFor(x => x.HighestQualification)
                .NotEmpty().WithMessage("Highest qualification record is required.");

            RuleFor(x => x.Address)
                .NotEmpty().WithMessage("An address record is required.");

            RuleFor(x => x.IsMfaEnabled)
                .NotNull();

            RuleFor(x => x.MFAType)
                .NotEmpty()
                .Must(type => type == "None" || type == "AuthenticatorApp" || type == "Email")
                .WithMessage("MFA Type must be either 'None', 'AuthenticatorApp', or 'Email'.");

            RuleFor(x => x.MFAType)
                .NotEqual("None")
                .When(x => x.IsMfaEnabled)
                .WithMessage("Please specify a valid MFA mechanism (AuthenticatorApp or Email) when Multi-Factor is enabled.");
        }
    }
}
