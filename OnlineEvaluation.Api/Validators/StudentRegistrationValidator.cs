using FluentValidation;
using OnlineEvaluation.Api.Models.DTO;

namespace OnlineEvaluation.Api.Validators
{
    public class StudentRegistrationValidator : AbstractValidator<StudentRegistrationDto>
    {
        private readonly string[] _allowedGenders = { "Male", "Female", "Other" };
        public StudentRegistrationValidator()
        {
            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("First Name is required.")
                .MaximumLength(100).WithMessage("First Name cannot exceed 100 characters.");

            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("Last Name is required.")
                .MaximumLength(100).WithMessage("Last Name cannot exceed 100 characters.");

            RuleFor(x => x.RegistrationNumber)
                .NotEmpty().WithMessage("Registration Number is required.")
                .MaximumLength(30).WithMessage("Registration Number cannot exceed 30 characters.")
                .Must(reg => reg.Length >= 2 && int.TryParse(reg.Substring(0, 2), out _))
                .WithMessage("Registration Number must start with a valid 2-digit year prefix (e.g., '26').");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email address is required.")
                .EmailAddress().WithMessage("Invalid email syntax format.")
                .MaximumLength(100).WithMessage("Email cannot exceed 100 characters.");

            // Validates password strength only when a password value is present
            RuleFor(x => x.Password)
                .MinimumLength(8).WithMessage("Password must be at least 6 characters long.")
                .MaximumLength(100).WithMessage("Password cannot exceed 100 characters.")
                .When(x => !string.IsNullOrEmpty(x.Password));

            RuleFor(x => x.AcademicMapId)
                .GreaterThan(0).WithMessage("A valid target Academic Map selection is required.");

            RuleFor(x => x.FatherName)
                .NotEmpty().WithMessage("Father's Name is required.")
                .MaximumLength(100).WithMessage("Father's Name cannot exceed 100 characters.");

            RuleFor(x => x.DateOfBirth)
                .NotEmpty().WithMessage("Date of Birth is required.")
                .LessThan(DateTime.Today).WithMessage("Date of birth cannot exist in the future.");

            RuleFor(x => x.Gender)
                .NotEmpty().WithMessage("Gender designation is required.")
                .MaximumLength(15).WithMessage("Gender entry is too long.")
                .Must(gender => _allowedGenders.Contains(gender, StringComparer.OrdinalIgnoreCase))
                .WithMessage($"Gender must be one of the following values: {string.Join(", ", _allowedGenders)}.");

            RuleFor(x => x.ContactNumber)
                .MaximumLength(10).WithMessage("Contact number cannot exceed 10 digits.");

            RuleFor(x => x.Address)
                .MaximumLength(500).WithMessage("Address details cannot exceed 500 characters.");

            RuleFor(x => x.BloodGroup)
                .MaximumLength(10).WithMessage("Blood Group indicator cannot exceed 10 characters.");
        }
    }
}
