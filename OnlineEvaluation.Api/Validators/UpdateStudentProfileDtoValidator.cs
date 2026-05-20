using FluentValidation;
using OnlineEvaluation.Api.Models.DTO;

namespace OnlineEvaluation.Api.Validators
{
    public class UpdateStudentProfileDtoValidator : AbstractValidator<UpdateStudentProfileDto>
    {
        private readonly List<string> _allowedBloodGroups = new()
        {
            "A+", "A-", "B+", "B-", "AB+", "AB-", "O+", "O-"
        };
        public UpdateStudentProfileDtoValidator()
        {
            RuleFor(x => x.ContactNumber)
                .NotEmpty().WithMessage("Contact number cannot be blank.")
                .Matches(@"^\+?[1-9]\d{1,14}$").WithMessage("Please provide a valid international phone format (e.g., +1234567890 or 10-15 digits).")
                .Length(10).WithMessage("Contact configurations require 10 digits.");

            RuleFor(x => x.Address)
                .NotEmpty().WithMessage("Residential address tracking is required.")
                .MaximumLength(500).WithMessage("Physical address blocks cannot exceed 500 characters total.");

            RuleFor(x => x.BloodGroup)
                .NotEmpty().WithMessage("Blood group selection cannot be blank.")
                .Must(bg => bg != null && _allowedBloodGroups.Contains(bg.ToUpper()))
                .WithMessage("Blood group must match standard formats (e.g., O+, A-, AB+).");
        }
    }
}
