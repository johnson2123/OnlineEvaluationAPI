using FluentValidation;
using OnlineEvaluation.Api.Models.DTO;

namespace OnlineEvaluation.Api.Validators
{
    public class CreateCollegeValidator : AbstractValidator<CreateCollegeDto>
    {
        private static readonly string[] AllowedStatuses = { "Active", "Inactive", "Suspended" };

        public CreateCollegeValidator()
        {
            // Mandatory Fields
            RuleFor(x => x.UniversityCode)
                .NotEmpty().WithMessage("University Code is required to link the college.")
                .MaximumLength(50);

            RuleFor(x => x.Code)
                .NotEmpty().WithMessage("College Code is required.")
                .Matches("^[A-Z0-9\\-]{2,50}$")
                .WithMessage("Code must be uppercase letters, numbers or hyphen (2-50 chars).");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required.")
                .MaximumLength(250);


            RuleFor(x => x.DisplayName).MaximumLength(250);
            RuleFor(x => x.Address).MaximumLength(500);
            RuleFor(x => x.City).MaximumLength(100);
            RuleFor(x => x.State).MaximumLength(100);
            RuleFor(x => x.Country).MaximumLength(100);
            RuleFor(x => x.PostalCode).MaximumLength(20);

            // Format Validations
            RuleFor(x => x.ContactEmail)
                .EmailAddress()
                .MaximumLength(254)
                .When(x => !string.IsNullOrWhiteSpace(x.ContactEmail));

            RuleFor(x => x.WebsiteUrl)
                .Must(uri => Uri.IsWellFormedUriString(uri, UriKind.Absolute))
                .MaximumLength(500)
                .When(x => !string.IsNullOrWhiteSpace(x.WebsiteUrl))
                .WithMessage("WebsiteUrl must be a valid absolute URL.");

            RuleFor(x => x.Status)
                .Must(s => string.IsNullOrEmpty(s) || AllowedStatuses.Contains(s))
                .WithMessage($"Status must be one of: {string.Join(", ", AllowedStatuses)}.");
        }
    }
}
