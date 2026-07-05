using MediatR;
using UrlShortener.Application.Common.Exceptions;
using UrlShortener.Domain.Repositories;
using UrlShortener.Domain.ValueObjects;

namespace UrlShortener.Application.Features.ShortUrls.GetByCode;

public sealed class GetShortUrlByCodeHandler
    : IRequestHandler<GetShortUrlByCodeQuery, GetShortUrlByCodeResponse>
{
    private readonly IShortUrlRepository _repository;

    public GetShortUrlByCodeHandler(IShortUrlRepository repository)
    {
        _repository = repository;
    }

    public async Task<GetShortUrlByCodeResponse> Handle(
        GetShortUrlByCodeQuery request,
        CancellationToken cancellationToken)
    {
        var shortCode = ShortCode.Create(request.ShortCode);

        var shortUrl = await _repository.GetByCodeAsync(
            shortCode,
            cancellationToken);

        if (shortUrl is null)
        {
            throw new NotFoundException("Short URL was not found.");
        }

        return new GetShortUrlByCodeResponse(
            shortUrl.ShortCode.Value,
            shortUrl.OriginalUrl.Value,
            shortUrl.CreatedAt,
            shortUrl.ExpiresAt,
            shortUrl.ClickCount,
            shortUrl.IsActive);
    }
}