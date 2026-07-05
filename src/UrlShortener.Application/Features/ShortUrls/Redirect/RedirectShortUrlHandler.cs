using MediatR;
using UrlShortener.Domain.Repositories;
using UrlShortener.Domain.ValueObjects;
using UrlShortener.Application.Common;
using UrlShortener.Application.Common.Exceptions;
using UrlShortener.Application.Abstractions.Services;

namespace UrlShortener.Application.Features.ShortUrls.Redirect;

public sealed class RedirectShortUrlHandler
    : IRequestHandler<RedirectShortUrlCommand, Result<string>>
{
    private readonly IShortUrlRepository _repository;
    private readonly IDateTimeProvider _dateTimeProvider;

    public RedirectShortUrlHandler(
        IShortUrlRepository repository,
        IDateTimeProvider dateTimeProvider)
    {
        _repository = repository;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<Result<string>> Handle(
        RedirectShortUrlCommand request,
        CancellationToken cancellationToken)
    {
        var shortCode = ShortCode.Create(request.ShortCode);

        var entity = await _repository.GetByCodeAsync(shortCode, cancellationToken);

        if (entity is null)
        {
            throw new NotFoundException("Short URL was not found.");
        }

        if (!entity.IsActive)
        {
            throw new NotFoundException("Short URL is inactive.");
        }

        entity.RegisterClick(_dateTimeProvider.UtcNow);
        await _repository.SaveChangesAsync(cancellationToken);

        return Result<string>.Success(entity.OriginalUrl.Value);
    }
}