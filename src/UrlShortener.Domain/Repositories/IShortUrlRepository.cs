using UrlShortener.Domain.Entities;
using UrlShortener.Domain.ValueObjects;

namespace UrlShortener.Domain.Repositories;

public interface IShortUrlRepository
{
    Task AddAsync(
        ShortUrl shortUrl,
        CancellationToken cancellationToken);

    Task<ShortUrl?> GetByCodeAsync(
        ShortCode shortCode,
        CancellationToken cancellationToken);

    Task<bool> ExistsByCodeAsync(
        ShortCode shortCode,
        CancellationToken cancellationToken);

    Task SaveChangesAsync(
        CancellationToken cancellationToken);
}