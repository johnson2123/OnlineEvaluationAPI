using FluentValidation;
using OnlineEvaluation.Api.Models.DTO;

namespace OnlineEvaluation.Api.Validators
{
    public class UpdateCollegeValidator : AbstractValidator<UpdateCollegeDto>
    {
        private static readonly string[] AllowedStatuses = { "Active", "Inactive", "Suspended" };

        public UpdateCollegeValidator()
        {
            // Only validate if the property is being updated (not null)

            RuleFor(x => x.UniversityCode)
                .NotEmpty()
                .MaximumLength(50)
                .When(x => x.UniversityCode != null);

            RuleFor(x => x.Code)
                .Matches("^[A-Z0-9\\-]{2,50}$")
                .WithMessage("Code must be uppercase letters, numbers or hyphen (2-50 chars).")
                .When(x => x.Code != null);

            RuleFor(x => x.Name)
                .NotEmpty()
                .MaximumLength(250)
                .When(x => x.Name != null);

            RuleFor(x => x.DisplayName)
                .MaximumLength(250)
                .When(x => x.DisplayName != null);

            RuleFor(x => x.Address)
                .MaximumLength(500)
                .When(x => x.Address != null);

            RuleFor(x => x.City)
                .MaximumLength(100)
                .When(x => x.City != null);

            RuleFor(x => x.State)
                .MaximumLength(100)
                .When(x => x.State != null);

            RuleFor(x => x.Country)
                .MaximumLength(100)
                .When(x => x.Country != null);

            RuleFor(x => x.PostalCode)
                .MaximumLength(20)
                .When(x => x.PostalCode != null);

            RuleFor(x => x.ContactEmail)
                .EmailAddress()
                .MaximumLength(254)
                .When(x => !string.IsNullOrWhiteSpace(x.ContactEmail));

            RuleFor(x => x.WebsiteUrl)
                .Must(uri => Uri.IsWellFormedUriString(uri, UriKind.Absolute))
                .WithMessage("WebsiteUrl must be a valid absolute URL.")
                .MaximumLength(500)
                .When(x => !string.IsNullOrWhiteSpace(x.WebsiteUrl));

            RuleFor(x => x.Status)
                .Must(s => AllowedStatuses.Contains(s))
                .WithMessage($"Status must be one of: {string.Join(", ", AllowedStatuses)}.")
                .When(x => x.Status != null);
        }
    }
}
