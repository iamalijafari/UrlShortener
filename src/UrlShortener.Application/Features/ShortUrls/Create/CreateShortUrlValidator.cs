using FluentValidation;

namespace UrlShortener.Application.Features.ShortUrls.Create;

public sealed class CreateShortUrlValidator
    : AbstractValidator<CreateShortUrlCommand>
{
    public CreateShortUrlValidator()
    {
        RuleFor(x => x.OriginalUrl)
            .NotEmpty()
            .Must(BeAValidUrl)
            .WithMessage("Invalid URL format");

        RuleFor(x => x.ExpiresAt)
            .Must(expiresAt =>
                !expiresAt.HasValue ||
                expiresAt.Value > DateTime.UtcNow)
            .WithMessage("Expiration must be in the future.");
    }

    private bool BeAValidUrl(string url)
    {
        return Uri.TryCreate(url, UriKind.Absolute, out _);
    }
}
