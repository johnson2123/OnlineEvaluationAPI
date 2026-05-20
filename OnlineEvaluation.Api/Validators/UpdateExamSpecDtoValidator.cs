using FluentValidation;
using OnlineEvaluation.Api.Models.DTO;

namespace OnlineEvaluation.Api.Validators
{
    public class UpdateExamSpecDtoValidator : AbstractValidator<UpdateExamSpecDto>
    {
        public UpdateExamSpecDtoValidator()
        {

            RuleFor(x => x.InternalMaxMarks)
                .InclusiveBetween(0, 100)
                .WithMessage("Internal maximum marks must be between 0 and 100.");

            RuleFor(x => x.ExternalMaxMarks)
                .InclusiveBetween(0, 100)
                .WithMessage("External maximum marks must be between 0 and 100.");


            RuleFor(x => x.TotalMaxMarks)
                .Equal(x => x.InternalMaxMarks + x.ExternalMaxMarks)
                .WithMessage(x => $"Updated Total Max Marks ({x.TotalMaxMarks}) must equal the sum of Internals ({x.InternalMaxMarks}) and Externals ({x.ExternalMaxMarks}).");

            RuleFor(x => x.ExternalPassingMarks)
                .GreaterThanOrEqualTo(0)
                .WithMessage("External passing marks cannot be negative.")
                .LessThanOrEqualTo(x => x.ExternalMaxMarks)
                .WithMessage("External passing marks cannot exceed maximum external marks.");

            RuleFor(x => x.TotalPassingMarks)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Total passing marks cannot be negative.")
                .LessThanOrEqualTo(x => x.TotalMaxMarks)
                .WithMessage("Total passing marks cannot exceed the total maximum marks bounds.")
                .Must((dto, totalPass) => totalPass >= dto.ExternalPassingMarks)
                .WithMessage("Total passing marks must be greater than or equal to the external passing marks.");
        }
    }
}
