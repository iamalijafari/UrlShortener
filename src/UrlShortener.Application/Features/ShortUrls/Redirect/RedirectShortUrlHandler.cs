using MediatR;
using UrlShortener.Domain.Repositories;
using UrlShortener.Domain.ValueObjects;
using UrlShortener.Application.Common;
using UrlShortener.Application.Common.Exceptions;
using UrlShortener.Application.Abstractions.Services;
using UrlShortener.Application.Abstractions.Messaging;
using UrlShortener.Application.Common.Observability;
using UrlShortener.Application.IntegrationEvents;

namespace UrlShortener.Application.Features.ShortUrls.Redirect;

public sealed class RedirectShortUrlHandler
    : IRequestHandler<RedirectShortUrlCommand, Result<string>>
{
    private readonly IShortUrlRepository _repository;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IRedirectCache _redirectCache;
    private readonly IUrlVisitOutbox _outbox;

    public RedirectShortUrlHandler(
        IShortUrlRepository repository,
        IDateTimeProvider dateTimeProvider,
        IRedirectCache redirectCache,
        IUrlVisitOutbox outbox)
    {
        _repository = repository;
        _dateTimeProvider = dateTimeProvider;
        _redirectCache = redirectCache;
        _outbox = outbox;
    }

    public async Task<Result<string>> Handle(
        RedirectShortUrlCommand request,
        CancellationToken cancellationToken)
    {
        using var activity = Telemetry.ActivitySource.StartActivity("redirect.resolve");
        activity?.SetTag("url.short_code", request.ShortCode);

        var now = _dateTimeProvider.UtcNow;
        var cached = await _redirectCache.GetAsync(
            request.ShortCode,
            cancellationToken);

        if (cached is not null)
        {
            activity?.SetTag("cache.hit", true);

            if (cached.ExpiresAtUtc.HasValue && cached.ExpiresAtUtc <= now)
            {
                await _redirectCache.RemoveAsync(request.ShortCode, cancellationToken);
                throw new NotFoundException("Short URL is expired.");
            }

            await PersistVisitAsync(
                cached.ShortUrlId,
                request.ShortCode,
                now,
                cancellationToken);

            return Result<string>.Success(cached.OriginalUrl);
        }

        activity?.SetTag("cache.hit", false);

        var shortCode = ShortCode.Create(request.ShortCode);
        var entity = await _repository.GetByCodeAsync(shortCode, cancellationToken);

        if (entity is null || !entity.CanRedirect(now))
        {
            throw new NotFoundException("Short URL is unavailable.");
        }

        await PersistVisitAsync(
            entity.Id,
            entity.ShortCode.Value,
            now,
            cancellationToken);

        await _redirectCache.SetAsync(
            entity.ShortCode.Value,
            new RedirectCacheEntry(
                entity.Id,
                entity.OriginalUrl.Value,
                entity.ExpiresAt),
            cancellationToken);

        return Result<string>.Success(entity.OriginalUrl.Value);
    }

    private async Task PersistVisitAsync(
        Guid shortUrlId,
        string shortCode,
        DateTime visitedAtUtc,
        CancellationToken cancellationToken)
    {
        _outbox.Add(new UrlVisitedIntegrationEvent(
            Guid.NewGuid(),
            shortUrlId,
            shortCode,
            visitedAtUtc));

        await _repository.SaveChangesAsync(cancellationToken);
    }
}
