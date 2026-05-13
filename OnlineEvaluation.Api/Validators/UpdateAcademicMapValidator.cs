using FluentValidation;
using OnlineEvaluation.Api.Models.DTO;

namespace OnlineEvaluation.Api.Validators
{
    public class UpdateAcademicMapValidator : AbstractValidator<UpdateAcademicMapDto>
    {
        public UpdateAcademicMapValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Entity ID is required for updates.");

            RuleFor(x => x.Guid)
                .NotEmpty().WithMessage("Security GUID is required.");

            RuleFor(x => x.CollegeId)
                .GreaterThan(0).WithMessage("Please select a valid College.");

            RuleFor(x => x.StudyProgramId)
                .GreaterThan(0).WithMessage("Please select a valid Program.");

            RuleFor(x => x.BranchId)
                .GreaterThan(0).WithMessage("Please select a valid Branch.");

            RuleFor(x => x.AliasCode)
                .MaximumLength(100).WithMessage("Alias Code is too long.");
        }
    }
}
