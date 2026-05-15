using FluentValidation;
using OnlineEvaluation.Api.Models.DTO;

namespace OnlineEvaluation.Api.Validators
{
    public class CreateAcademicMapValidator : AbstractValidator<CreateAcademicMapDto>
    {
        public CreateAcademicMapValidator()
        {
            RuleFor(x => x.CollegeId)
                .NotEmpty().WithMessage("Please select a College.")
                .GreaterThan(0).WithMessage("Invalid College selection.");

            RuleFor(x => x.StudyProgramId)
                .NotEmpty().WithMessage("Please select a Study Program.")
                .GreaterThan(0).WithMessage("Invalid Program selection.");

            RuleFor(x => x.BranchId)
                .NotEmpty().WithMessage("Please select a Branch.")
                .GreaterThan(0).WithMessage("Invalid Branch selection.");

            RuleFor(x => x.Regulation)
                .NotEmpty().WithMessage("Regulation code is required.")
                .MaximumLength(20).WithMessage("Regulation code cannot be longer than 20 characters.")
                .Matches(@"^[a-zA-Z0-9-]+$").WithMessage("Regulation can only contain letters, numbers, and hyphens (e.g., R20, R-23).");

        }
    }
}
