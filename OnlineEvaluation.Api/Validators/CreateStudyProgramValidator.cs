using FluentValidation;
using OnlineEvaluation.Api.Models.DTO;

namespace OnlineEvaluation.Api.Validators
{
    public class CreateStudyProgramValidator : AbstractValidator<CreateStudyProgramDto>
    {
        public CreateStudyProgramValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Program Name is required.")
                .MaximumLength(200).WithMessage("Program Name cannot exceed 200 characters.");

            RuleFor(x => x.ShortName)
                .NotEmpty().WithMessage("Short Name (e.g., B.Tech) is required.")
                .MaximumLength(50).WithMessage("Short Name cannot exceed 50 characters.");

            // Strict Enum check: Must be one of UG, PG, PhD, etc.
            RuleFor(x => x.Level)
                .IsInEnum().WithMessage("Please select a valid Academic Level (UG, PG, PhD, etc.).");

            RuleFor(x => x.DurationInYears)
                .InclusiveBetween(1, 6).WithMessage("Duration must be between 1 and 6 years.");

            RuleFor(x => x.TotalSemesters)
                .InclusiveBetween(1, 12).WithMessage("Total semesters must be between 1 and 12.")
                .Must((dto, semesters) => semesters >= dto.DurationInYears)
                .WithMessage("Total semesters cannot be less than the duration in years.");
        }
    }
}
