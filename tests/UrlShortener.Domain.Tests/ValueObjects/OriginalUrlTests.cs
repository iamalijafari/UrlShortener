using FluentAssertions;
using UrlShortener.Domain.Exceptions;
using UrlShortener.Domain.ValueObjects;

namespace UrlShortener.Domain.Tests.ValueObjects;

public class OriginalUrlTests
{
    [Fact]
    public void Create_Should_Create_ValueObject_When_Url_Is_Valid()
    {
        // Arrange
        const string url = "https://google.com";

        // Act
        var result = OriginalUrl.Create(url);

        // Assert
        result.Value.Should().Be(url);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("not-a-url")]
    [InlineData("ftp://google.com")]
    public void Create_Should_Throw_When_Url_Is_Invalid(string url)
    {
        // Act
        var action = () => OriginalUrl.Create(url);

        // Assert
        action.Should()
              .Throw<DomainException>();
    }
}