namespace UrlShortener.Api.Tests.Common;

[CollectionDefinition(Name)]
public sealed class IntegrationTestCollection
    : ICollectionFixture<CustomWebApplicationFactory>
{
    public const string Name = "Integration";
}
