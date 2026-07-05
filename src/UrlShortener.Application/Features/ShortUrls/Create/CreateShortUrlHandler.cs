using MediatR;
using UrlShortener.Domain.Entities;
using UrlShortener.Domain.Repositories;
using UrlShortener.Domain.ValueObjects;
using UrlShortener.Application.Abstractions.Services;

namespace UrlShortener.Application.Features.ShortUrls.Create;

public sealed class CreateShortUrlHandler
    : IRequestHandler<CreateShortUrlCommand, CreateShortUrlResponse>
{
    private readonly IShortUrlRepository _repository;
    private readonly IShortCodeGenerator _codeGenerator;
    private readonly IDateTimeProvider _dateTimeProvider;

    public CreateShortUrlHandler(
        IShortUrlRepository repository,
        IShortCodeGenerator codeGenerator,
        IDateTimeProvider dateTimeProvider)
    {
        _repository = repository;
        _codeGenerator = codeGenerator;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<CreateShortUrlResponse> Handle(
        CreateShortUrlCommand request,
        CancellationToken cancellationToken)
    {
        var originalUrl = OriginalUrl.Create(request.OriginalUrl);

        string code;
        ShortCode shortCode;

        do
        {
            code = _codeGenerator.Generate();
            shortCode = ShortCode.Create(code);

        } while (await _repository.ExistsByCodeAsync(shortCode, cancellationToken));

        var now = _dateTimeProvider.UtcNow;

        var shortUrl = new ShortUrl(
            originalUrl,
            shortCode,
            now,
            now.AddDays(5));

        await _repository.AddAsync(shortUrl, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);

        return new CreateShortUrlResponse(
            shortCode.Value,
            $"http://localhost:5000/{shortCode.Value}");
    }
}