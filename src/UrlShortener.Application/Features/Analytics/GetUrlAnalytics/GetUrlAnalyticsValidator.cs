using FluentValidation;

namespace UrlShortener.Application.Features.Analytics.GetUrlAnalytics;

public sealed class GetUrlAnalyticsValidator : AbstractValidator<GetUrlAnalyticsQuery>
{
    public GetUrlAnalyticsValidator()
    {
        RuleFor(x => x.ShortCode)
            .NotEmpty()
            .MinimumLength(3)
            .MaximumLength(12);

        RuleFor(x => x)
            .Must(x => !x.From.HasValue || !x.To.HasValue || x.From <= x.To)
            .WithMessage("'from' must be on or before 'to'.");

        RuleFor(x => x)
            .Must(x => !x.From.HasValue || !x.To.HasValue ||
                x.To.Value.DayNumber - x.From.Value.DayNumber <= 366)
            .WithMessage("The analytics range cannot exceed 366 days.");
    }
}
