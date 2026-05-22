using FluentValidation;
using OnlineEvaluation.Api.Models.DTO;

namespace OnlineEvaluation.Api.Validators
{
    public class CreateCollegeDepartmentValidator : AbstractValidator<CreateCollegeDepartmentDto>
    {
        public CreateCollegeDepartmentValidator()
        {
            RuleFor(x => x.CollegeId)
                .GreaterThan(0)
                .WithMessage("Please select a valid College from the list.");

            RuleFor(x => x.DepartmentId)
                .GreaterThan(0)
                .WithMessage("Please select a valid Department from the list.");
        }
    }
}
