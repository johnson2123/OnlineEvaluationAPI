using FluentValidation;
using OnlineEvaluation.Api.Models.DTO;

namespace OnlineEvaluation.Api.Validators
{
    public class CreateBranchValidator : AbstractValidator<CreateBranchDto>
    {
        public CreateBranchValidator()
        {
            RuleFor(x => x.Code)
                .NotEmpty().WithMessage("Branch code is required.")
                .MaximumLength(20).WithMessage("Code cannot exceed 20 characters.")
                .Matches(@"^[^\s]+$").WithMessage("Code cannot contain spaces (e.g., use 'CSE' or 'CSE-AI').");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Branch name is required.")
                .MaximumLength(200).WithMessage("Name cannot exceed 200 characters.");

            RuleFor(x => x.DisplayName)
                .MaximumLength(250).WithMessage("Display Name cannot exceed 250 characters.");
        }
    }
}
