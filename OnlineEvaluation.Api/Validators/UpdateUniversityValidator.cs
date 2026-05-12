using FluentValidation;
using OnlineEvaluation.Api.Models.DTO;
namespace OnlineEvaluation.Api.Validators
{
    public class UpdateUniversityValidator : AbstractValidator<UpdateUniversityDto>
    {
        private static readonly string[] AllowedStatuses = { "Active", "Inactive", "Suspended", "Pending" };

        public UpdateUniversityValidator()
        {
            When(x => x.Name != null, () =>
            {
                RuleFor(x => x.Name)
                    .NotEmpty().WithMessage("Name cannot be empty when provided.")
                    .Length(3, 250);
            });

            When(x => x.DisplayName != null, () =>
            {
                RuleFor(x => x.DisplayName).MaximumLength(250);
            });

            When(x => x.Code != null, () =>
            {
                RuleFor(x => x.Code)
                    .Matches("^[A-Z0-9\\-]{2,50}$")
                    .WithMessage("Code must be uppercase letters, numbers or hyphen, 2-50 chars.");
            });

            When(x => x.ContactEmail != null, () =>
            {
                RuleFor(x => x.ContactEmail)
                    .EmailAddress()
                    .MaximumLength(254);
            });

            When(x => x.WebsiteUrl != null, () =>
            {
                RuleFor(x => x.WebsiteUrl)
                    .Must(uri => Uri.IsWellFormedUriString(uri, UriKind.Absolute))
                    .MaximumLength(500)
                    .WithMessage("WebsiteUrl must be a valid absolute URL.");
            });

            RuleFor(x => x.Status)
                .Must(s => s == null || AllowedStatuses.Contains(s))
                .WithMessage($"Status must be one of: {string.Join(", ", AllowedStatuses)}.");
        }
    }
}