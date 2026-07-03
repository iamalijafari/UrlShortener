using Microsoft.EntityFrameworkCore;
using UrlShortener.Domain.Entities;
using UrlShortener.Domain.Repositories;
using UrlShortener.Domain.ValueObjects;

namespace UrlShortener.Infrastructure.Persistence.Repositories;

public sealed class ShortUrlRepository : IShortUrlRepository
{
    private readonly AppDbContext _context;

    public ShortUrlRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(ShortUrl shortUrl, CancellationToken cancellationToken)
    {
        await _context.ShortUrls.AddAsync(shortUrl, cancellationToken);
    }

    public async Task<ShortUrl?> GetByCodeAsync(ShortCode shortCode, CancellationToken cancellationToken)
    {
        return await _context.ShortUrls
            .FirstOrDefaultAsync(x => x.ShortCode == shortCode, cancellationToken);
    }

    public async Task<bool> ExistsByCodeAsync(ShortCode shortCode, CancellationToken cancellationToken)
    {
        return await _context.ShortUrls
            .AnyAsync(x => x.ShortCode == shortCode, cancellationToken);
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        return _context.SaveChangesAsync(cancellationToken);
    }
}