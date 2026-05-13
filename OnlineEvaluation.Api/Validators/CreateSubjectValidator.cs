using FluentValidation;
using OnlineEvaluation.Api.Constants;
using OnlineEvaluation.Api.Models.DTO;

namespace OnlineEvaluation.Api.Validators
{
    public class CreateSubjectValidator : AbstractValidator<CreateSubjectDto>
    {
        public CreateSubjectValidator()
        {
            RuleFor(x => x.Code)
                .NotEmpty().WithMessage("Subject Code is required.")
                .MaximumLength(20).WithMessage("Code cannot exceed 20 characters.")
                .Matches(@"^[a-zA-Z0-9-]*$").WithMessage("Code can only contain alphanumeric characters and hyphens.");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Subject Name is required.")
                .MaximumLength(250).WithMessage("Name cannot exceed 250 characters.");

            RuleFor(x => x.DisplayName)
                .MaximumLength(100).WithMessage("Display Name cannot exceed 100 characters.");

            RuleFor(x => x.Credits)
                .InclusiveBetween(0, 15).WithMessage("Credits must be between 0 and 15.");

            // Validates that the value (int or string) maps to the SubjectType Enum
            RuleFor(x => x.Type)
                .IsInEnum().WithMessage("Please select a valid Subject Type (Theory, Practical, etc.).");

            RuleFor(x => x.Description)
                .MaximumLength(500).WithMessage("Description cannot exceed 500 characters.");
        }
    }
}
