using MediatR;
using UrlShortener.Application.Common;

namespace UrlShortener.Application.Features.ShortUrls.Redirect;

public sealed record RedirectShortUrlCommand(
    string ShortCode
) : IRequest<Result<string>>;