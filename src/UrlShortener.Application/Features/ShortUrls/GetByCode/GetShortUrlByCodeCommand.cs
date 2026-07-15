using MediatR;

namespace UrlShortener.Application.Features.ShortUrls.GetByCode;

public sealed record GetShortUrlByCodeCommand(
    string ShortCode
) : IRequest<GetShortUrlByCodeResponse>;