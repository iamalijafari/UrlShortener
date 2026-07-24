using MediatR;
using UrlShortener.Application.Abstractions.Services;
using UrlShortener.Application.Common.Exceptions;
using UrlShortener.Domain.Repositories;
using UrlShortener.Domain.ValueObjects;

namespace UrlShortener.Application.Features.Analytics.GetUrlAnalytics;

public sealed class GetUrlAnalyticsHandler
    : IRequestHandler<GetUrlAnalyticsQuery, GetUrlAnalyticsResponse>
{
    private readonly IShortUrlRepository _shortUrlRepository;
    private readonly IUrlAnalyticsReader _analyticsReader;
    private readonly IDateTimeProvider _dateTimeProvider;

    public GetUrlAnalyticsHandler(
        IShortUrlRepository shortUrlRepository,
        IUrlAnalyticsReader analyticsReader,
        IDateTimeProvider dateTimeProvider)
    {
        _shortUrlRepository = shortUrlRepository;
        _analyticsReader = analyticsReader;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<GetUrlAnalyticsResponse> Handle(
        GetUrlAnalyticsQuery request,
        CancellationToken cancellationToken)
    {
        var shortCode = ShortCode.Create(request.ShortCode);
        var shortUrl = await _shortUrlRepository.GetByCodeAsync(
            shortCode,
            cancellationToken);

        if (shortUrl is null)
        {
            throw new NotFoundException("Short URL was not found.");
        }

        var today = DateOnly.FromDateTime(_dateTimeProvider.UtcNow);
        var from = request.From ?? today.AddDays(-29);
        var to = request.To ?? today;

        var visits = await _analyticsReader.GetDailyVisitsAsync(
            shortUrl.Id,
            from,
            to,
            cancellationToken);

        var visitsByDate = visits.ToDictionary(x => x.Date, x => x.ClickCount);
        var daily = Enumerable.Range(0, to.DayNumber - from.DayNumber + 1)
            .Select(offset =>
            {
                var date = from.AddDays(offset);
                return new DailyUrlAnalytics(
                    date,
                    visitsByDate.GetValueOrDefault(date));
            })
            .ToArray();

        return new GetUrlAnalyticsResponse(
            shortUrl.ShortCode.Value,
            from,
            to,
            daily.Sum(x => x.ClickCount),
            daily);
    }
}
