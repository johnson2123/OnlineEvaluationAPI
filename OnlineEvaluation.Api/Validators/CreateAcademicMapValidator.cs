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

            RuleFor(x => x.AliasCode)
                .MaximumLength(100).WithMessage("Alias Code cannot be longer than 100 characters.");


        }
    }
}
