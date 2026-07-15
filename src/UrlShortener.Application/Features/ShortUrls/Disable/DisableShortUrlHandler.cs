using MediatR;
using UrlShortener.Application.Common.Exceptions;
using UrlShortener.Domain.Repositories;
using UrlShortener.Domain.ValueObjects;

namespace UrlShortener.Application.Features.ShortUrls.Disable;

public sealed class DisableShortUrlHandler
    : IRequestHandler<DisableShortUrlCommand>
{
    private readonly IShortUrlRepository _repository;
    public DisableShortUrlHandler(IShortUrlRepository repository)
    {
        _repository = repository;
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
    }
}