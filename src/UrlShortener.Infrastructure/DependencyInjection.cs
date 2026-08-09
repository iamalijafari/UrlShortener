using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using UrlShortener.Application.Abstractions.Services;
using UrlShortener.Domain.Repositories;
using UrlShortener.Infrastructure.Persistence;
using UrlShortener.Infrastructure.Persistence.Repositories;
using UrlShortener.Infrastructure.Services;
using StackExchange.Redis;
using UrlShortener.Application.Abstractions.Messaging;
using UrlShortener.Infrastructure.Caching;
using UrlShortener.Infrastructure.Messaging;
using UrlShortener.Infrastructure.Persistence.Analytics;
using UrlShortener.Infrastructure.Persistence.Outbox;

namespace UrlShortener.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(
                configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<IShortUrlRepository, ShortUrlRepository>();
        services.AddScoped<IUrlVisitRecorder, UrlVisitRecorder>();
        services.AddScoped<IUrlAnalyticsReader, UrlAnalyticsReader>();
        services.AddScoped<IUrlVisitedEventProcessor, UrlVisitedEventProcessor>();

        services.AddScoped<IShortCodeGenerator, Base62ShortCodeGenerator>();
        services.AddScoped<IDateTimeProvider, DateTimeProvider>();

        services.Configure<RedisOptions>(
            configuration.GetSection(RedisOptions.SectionName));
        services.AddSingleton<IConnectionMultiplexer>(_ =>
        {
            var redisConfiguration = ConfigurationOptions.Parse(
                configuration[$"{RedisOptions.SectionName}:ConnectionString"]
                ?? "localhost:6379");
            redisConfiguration.AbortOnConnectFail = false;
            return ConnectionMultiplexer.Connect(redisConfiguration);
        });
        services.AddScoped<IRedirectCache, RedisRedirectCache>();

        services.Configure<RabbitMqOptions>(
            configuration.GetSection(RabbitMqOptions.SectionName));
        services.AddSingleton<IRabbitMqConnection, RabbitMqConnection>();
        services.AddSingleton<IIntegrationEventPublisher, RabbitMqIntegrationEventPublisher>();

        return services;
    }
}
