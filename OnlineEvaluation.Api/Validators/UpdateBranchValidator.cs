using FluentValidation;
using OnlineEvaluation.Api.Models.DTO;

namespace OnlineEvaluation.Api.Validators
{
    public class UpdateBranchValidator : AbstractValidator<UpdateBranchDto>
    {
        public UpdateBranchValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Id is required for updates.");

            RuleFor(x => x.Code)
                .NotEmpty().WithMessage("Branch code is required.")
                .MaximumLength(20).WithMessage("Code cannot exceed 20 characters.")
                .Matches(@"^[^\s]+$").WithMessage("Code cannot contain spaces.");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Branch name is required.")
                .MaximumLength(200).WithMessage("Name cannot exceed 200 characters.");
        }
    }
}
