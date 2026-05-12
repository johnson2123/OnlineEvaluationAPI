using FluentValidation;
using OnlineEvaluation.Api.Models.DTO;

namespace OnlineEvaluation.Api.Validators
{
    public class CreateUniversityValidator : AbstractValidator<CreateUniversityDto>
    {
        private static readonly string[] AllowedStatuses = { "Active", "Inactive", "Suspended", "Pending" };

        public CreateUniversityValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required.")
                .MaximumLength(250).WithMessage("Name cannot exceed 250 characters.");


            RuleFor(x => x.Code)
                .NotEmpty().WithMessage("Code is required.")
                .Matches("^[A-Z0-9\\-]{2,50}$")
                .WithMessage("Code must be uppercase letters, numbers or hyphen, 2-50 chars.");


            RuleFor(x => x.DisplayName)
                .MaximumLength(250)
                .When(x => !string.IsNullOrWhiteSpace(x.DisplayName));

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
                .Must(s => s == null || AllowedStatuses.Contains(s))
                .WithMessage($"Status must be one of: {string.Join(", ", AllowedStatuses)}.");
        }
    }
}