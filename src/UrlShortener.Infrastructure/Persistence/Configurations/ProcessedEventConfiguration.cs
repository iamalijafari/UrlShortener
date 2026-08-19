using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UrlShortener.Infrastructure.Persistence.Models;

namespace UrlShortener.Infrastructure.Persistence.Configurations;

public sealed class ProcessedEventConfiguration
    : IEntityTypeConfiguration<ProcessedEvent>
{
    public void Configure(EntityTypeBuilder<ProcessedEvent> builder)
    {
        builder.ToTable("processed_events");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.ProcessedAtUtc).IsRequired();
    }
}
