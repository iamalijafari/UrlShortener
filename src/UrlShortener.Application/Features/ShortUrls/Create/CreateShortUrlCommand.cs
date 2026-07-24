using MediatR;

namespace UrlShortener.Application.Features.ShortUrls.Create;

public sealed record CreateShortUrlCommand(
    string OriginalUrl,
    DateTime? ExpiresAt = null
) : IRequest<CreateShortUrlResponse>;
