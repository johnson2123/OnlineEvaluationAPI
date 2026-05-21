using FluentValidation;
using OnlineEvaluation.Api.Models.DTO;

namespace OnlineEvaluation.Api.Validators
{
    public class UpdateDepartmentValidator : AbstractValidator<UpdateDepartmentDto>
    {
        public UpdateDepartmentValidator()
        {
            RuleFor(d => d.Code)
                .NotEmpty().WithMessage("Department code is required.")
                .MaximumLength(20).WithMessage("Code cannot exceed 20 characters.")
                .Matches(@"^[A-Z0-9\s\-]+$").WithMessage("Code must be alphanumeric and uppercase.");

            RuleFor(d => d.Name)
                .NotEmpty().WithMessage("Department name is required.")
                .MaximumLength(200).WithMessage("Name cannot exceed 200 characters.");

            RuleFor(d => d.DisplayName)
                .MaximumLength(250).WithMessage("Display name cannot exceed 250 characters.");

            RuleFor(d => d.Description)
                .MaximumLength(500).WithMessage("Description cannot exceed 500 characters.");
        }
    }
}
