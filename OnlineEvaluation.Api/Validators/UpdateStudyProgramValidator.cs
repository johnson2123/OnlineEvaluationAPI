using FluentValidation;
using OnlineEvaluation.Api.Models.DTO;

namespace OnlineEvaluation.Api.Validators
{
    public class UpdateStudyProgramValidator : AbstractValidator<UpdateStudyProgramDto>
    {
        public UpdateStudyProgramValidator()
        {
            // ID is mandatory for updates
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Program ID is required for updates.");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Program Name is required.")
                .MaximumLength(200);

            RuleFor(x => x.ShortName)
                .NotEmpty().WithMessage("Short Name is required.")
                .MaximumLength(50);

            RuleFor(x => x.Level)
                .IsInEnum().WithMessage("Please select a valid Academic Level.");

            RuleFor(x => x.DurationInYears)
                .InclusiveBetween(1, 6);

            RuleFor(x => x.TotalSemesters)
                .InclusiveBetween(1, 12);
        }
    }
}
