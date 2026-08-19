using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UrlShortener.Domain.Entities;
using UrlShortener.Infrastructure.Persistence.Models;

namespace UrlShortener.Infrastructure.Persistence.Configurations;

public sealed class DailyVisitStatisticConfiguration
    : IEntityTypeConfiguration<DailyVisitStatistic>
{
    public void Configure(EntityTypeBuilder<DailyVisitStatistic> builder)
    {
        builder.ToTable("daily_visit_statistics");
        builder.HasKey(x => new { x.ShortUrlId, x.Date });
        builder.Property(x => x.Date).HasColumnType("date");
        builder.Property(x => x.ClickCount).IsRequired();
        builder.Property(x => x.LastVisitedAtUtc).IsRequired();

        builder.HasOne<ShortUrl>()
            .WithMany()
            .HasForeignKey(x => x.ShortUrlId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
