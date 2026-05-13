using FluentValidation;
using OnlineEvaluation.Api.Constants;
using OnlineEvaluation.Api.Models.DTO;

namespace OnlineEvaluation.Api.Validators
{
    public class UpdateSubjectValidator : AbstractValidator<UpdateSubjectDto>
    {
        public UpdateSubjectValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Internal ID is required for updates.");

            RuleFor(x => x.Code)
                .NotEmpty().WithMessage("Subject Code is required.")
                .MaximumLength(20);

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Subject Name is required.")
                .MaximumLength(250);

            RuleFor(x => x.Type)
                .IsInEnum().WithMessage("Invalid Subject Type selected.");

            RuleFor(x => x.Credits)
                .InclusiveBetween(0, 15);
        }
    }
}
