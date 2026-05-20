using FluentValidation;
using OnlineEvaluation.Api.Models.DTO;

namespace OnlineEvaluation.Api.Validators
{
    public class CreateExamSpecDtoValidator : AbstractValidator<CreateExamSpecDto>
    {
        public CreateExamSpecDtoValidator()
        {

            RuleFor(x => x.AcademicMapId)
                .GreaterThan(0)
                .WithMessage("A valid Academic Map configuration must be selected.");

            RuleFor(x => x.SubjectId)
                .GreaterThan(0)
                .WithMessage("A valid Subject must be selected from the master ledger.");


            RuleFor(x => x.Semester)
                .InclusiveBetween(1, 8)
                .WithMessage("Semester must be a valid academic term between 1 and 8.");


            RuleFor(x => x.InternalMaxMarks)
                .InclusiveBetween(0, 100)
                .WithMessage("Internal maximum marks must be between 0 and 100.");

            RuleFor(x => x.ExternalMaxMarks)
                .InclusiveBetween(0, 100)
                .WithMessage("External maximum marks must be between 0 and 100.");


            RuleFor(x => x.TotalMaxMarks)
                .Equal(x => x.InternalMaxMarks + x.ExternalMaxMarks)
                .WithMessage(x => $"Total Max Marks ({x.TotalMaxMarks}) must exactly equal the sum of Internals ({x.InternalMaxMarks}) and Externals ({x.ExternalMaxMarks}).");


            RuleFor(x => x.ExternalPassingMarks)
                .GreaterThanOrEqualTo(0)
                .WithMessage("External passing marks cannot be a negative value.")
                .LessThanOrEqualTo(x => x.ExternalMaxMarks)
                .WithMessage("External passing marks cannot exceed the allocated maximum external marks.");

            RuleFor(x => x.TotalPassingMarks)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Total passing marks cannot be a negative value.")
                .LessThanOrEqualTo(x => x.TotalMaxMarks)
                .WithMessage("Total passing marks cannot exceed the maximum total marks capacity.")
                .Must((dto, totalPass) => totalPass >= dto.ExternalPassingMarks)
                .WithMessage("Total passing marks cannot be lower than the standalone external passing marks constraint.");
        }
    }
}
