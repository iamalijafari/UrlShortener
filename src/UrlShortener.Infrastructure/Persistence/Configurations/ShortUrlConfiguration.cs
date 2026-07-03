using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UrlShortener.Domain.Entities;
using UrlShortener.Domain.ValueObjects;

namespace UrlShortener.Infrastructure.Persistence.Configurations;

public sealed class ShortUrlConfiguration : IEntityTypeConfiguration<ShortUrl>
{
    public void Configure(EntityTypeBuilder<ShortUrl> builder)
    {
        builder.ToTable("short_urls");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.Property(x => x.ClickCount)
            .IsRequired();

        builder.Property(x => x.IsActive)
            .IsRequired();

        builder.Property(x => x.ExpiresAt);

        // Value Object: OriginalUrl
        builder.Property(x => x.OriginalUrl)
            .HasConversion(
                v => v.Value,
                v => OriginalUrl.Create(v))
            .HasMaxLength(2048)
            .IsRequired();

        // Value Object: ShortCode
        builder.Property(x => x.ShortCode)
            .HasConversion(
                v => v.Value,
                v => ShortCode.Create(v))
            .HasMaxLength(12)
            .IsRequired();

        builder.HasIndex(x => x.ShortCode)
            .IsUnique();
    }
}