using FluentValidation;
using OnlineEvaluation.Api.Models.DTO;

namespace OnlineEvaluation.Api.Validators
{
    public class UpdateAcademicMapValidator : AbstractValidator<UpdateAcademicMapDto>
    {
        public UpdateAcademicMapValidator()
        {

            RuleFor(x => x.CollegeId)
                .GreaterThan(0).WithMessage("Please select a valid College.");

            RuleFor(x => x.StudyProgramId)
                .GreaterThan(0).WithMessage("Please select a valid Program.");

            RuleFor(x => x.BranchId)
                .GreaterThan(0).WithMessage("Please select a valid Branch.");

            RuleFor(x => x.Regulation)
                .NotEmpty().WithMessage("Regulation code is required.")
                .MaximumLength(20).WithMessage("Regulation code cannot be longer than 20 characters.")
                .Matches(@"^[a-zA-Z0-9-]+$").WithMessage("Regulation can only contain letters, numbers, and hyphens (e.g., R20, R-23).");

        }
    }
}
