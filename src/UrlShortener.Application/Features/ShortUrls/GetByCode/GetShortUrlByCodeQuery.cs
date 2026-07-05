using MediatR;

namespace UrlShortener.Application.Features.ShortUrls.GetByCode;

public sealed record GetShortUrlByCodeQuery(
    string ShortCode
) : IRequest<GetShortUrlByCodeResponse>;