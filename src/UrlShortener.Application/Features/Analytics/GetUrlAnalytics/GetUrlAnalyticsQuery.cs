using MediatR;

namespace UrlShortener.Application.Features.Analytics.GetUrlAnalytics;

public sealed record GetUrlAnalyticsQuery(
    string ShortCode,
    DateOnly? From,
    DateOnly? To) : IRequest<GetUrlAnalyticsResponse>;
