using UrlShortener.Domain.Entities;

namespace UrlShortener.Domain.Repositories;

public interface IShortUrlRepository
{
    Task AddAsync(
        ShortUrl shortUrl,
        CancellationToken cancellationToken);

    Task<ShortUrl?> GetByCodeAsync(
        string shortCode,
        CancellationToken cancellationToken);

    Task<bool> ExistsByCodeAsync(
        string shortCode,
        CancellationToken cancellationToken);

    Task SaveChangesAsync(
        CancellationToken cancellationToken);
}