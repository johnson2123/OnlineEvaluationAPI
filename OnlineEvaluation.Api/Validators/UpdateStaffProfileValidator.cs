using FluentValidation;
using OnlineEvaluation.Api.Models.DTO;

namespace OnlineEvaluation.Api.Validators
{
    public class UpdateStaffProfileValidator : AbstractValidator<UpdateStaffProfileDto>
    {
        public UpdateStaffProfileValidator()
        {
            RuleFor(x => x.PhoneNumber)
                .MaximumLength(10).WithMessage("Phone number cannot exceed 10 characters.")
                .Matches(@"^\+?[0-9\s\-]+$").WithMessage("Invalid phone number format.")
                .When(x => !string.IsNullOrEmpty(x.PhoneNumber)); // Only validate if a phone number is provided

            RuleFor(x => x.Address)
                .NotEmpty().WithMessage("Address details are required.")
                .MaximumLength(500).WithMessage("Address details cannot exceed 500 characters.");
        }
    }
}
