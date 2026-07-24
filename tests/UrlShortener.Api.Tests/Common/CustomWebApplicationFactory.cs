using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;

namespace UrlShortener.Api.Tests.Common;

public sealed class CustomWebApplicationFactory
    : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly InfrastructureFixture _infrastructure = new();

    public string RedisConnectionString =>
        _infrastructure.RedisConnectionString;

    public string RabbitMqConnectionString =>
        _infrastructure.RabbitMqConnectionString;

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Development");
        builder.UseSetting(
            "ConnectionStrings:DefaultConnection",
            _infrastructure.PostgreSqlConnectionString);
        builder.UseSetting(
            "Redis:ConnectionString",
            _infrastructure.RedisConnectionString);
        builder.UseSetting(
            "RabbitMq:ConnectionString",
            _infrastructure.RabbitMqConnectionString);
        builder.UseSetting("OpenTelemetry:Endpoint", string.Empty);
    }

    public Task InitializeAsync() => _infrastructure.InitializeAsync();

    public new async Task DisposeAsync()
    {
        await base.DisposeAsync();
        await _infrastructure.DisposeAsync();
    }

    public HttpClient CreateNoRedirectClient()
    {
        return CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });
    }
}
