using MediatR;
using UrlShortener.Application.Common.Exceptions;
using UrlShortener.Domain.Repositories;
using UrlShortener.Domain.ValueObjects;
using UrlShortener.Application.Abstractions.Services;

namespace UrlShortener.Application.Features.ShortUrls.Disable;

public sealed class DisableShortUrlHandler
    : IRequestHandler<DisableShortUrlCommand>
{
    private readonly IShortUrlRepository _repository;
    private readonly IRedirectCache _redirectCache;

    public DisableShortUrlHandler(
        IShortUrlRepository repository,
        IRedirectCache redirectCache)
    {
        _repository = repository;
        _redirectCache = redirectCache;
    }

    public async Task Handle(DisableShortUrlCommand request, CancellationToken cancellationToken)
    {
        var shortCode = ShortCode.Create(request.ShortCode);

        var shortUrl = await _repository.GetByCodeAsync(
            shortCode,
            cancellationToken);

        if (shortUrl is null)
        {
            throw new NotFoundException("Short URL was not found.");
        }

        shortUrl.Disable();

        await _repository.SaveChangesAsync(cancellationToken);
        await _redirectCache.RemoveAsync(request.ShortCode, cancellationToken);
    }
}
