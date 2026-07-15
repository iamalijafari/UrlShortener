using MediatR;

namespace UrlShortener.Application.Features.ShortUrls.Disable;

public sealed record DisableShortUrlCommand(
    string ShortCode
) : IRequest;