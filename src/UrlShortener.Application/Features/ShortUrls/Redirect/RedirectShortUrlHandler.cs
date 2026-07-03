using MediatR;
using UrlShortener.Domain.Repositories;
using UrlShortener.Domain.ValueObjects;
using UrlShortener.Application.Common;

namespace UrlShortener.Application.Features.ShortUrls.Redirect;

public sealed class RedirectShortUrlHandler
    : IRequestHandler<RedirectShortUrlCommand, Result<string>>
{
    private readonly IShortUrlRepository _repository;

    public RedirectShortUrlHandler(IShortUrlRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<string>> Handle(
        RedirectShortUrlCommand request,
        CancellationToken cancellationToken)
    {
        var shortCode = ShortCode.Create(request.ShortCode);

        var entity = await _repository.GetByCodeAsync(shortCode, cancellationToken);

        if (entity is null)
            return Result<string>.Failure("Not found");

        if (!entity.IsActive)
            return Result<string>.Failure("Inactive link");

        entity.RegisterClick();
        await _repository.SaveChangesAsync(cancellationToken);

        return Result<string>.Success(entity.OriginalUrl.Value);
    }
}