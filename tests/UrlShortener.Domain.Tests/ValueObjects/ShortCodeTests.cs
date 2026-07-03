using FluentAssertions;
using UrlShortener.Domain.Exceptions;
using UrlShortener.Domain.ValueObjects;

namespace UrlShortener.Domain.Tests.ValueObjects;

public class ShortCodeTests
{
    [Fact]
    public void Create_Should_Create_ShortCode_When_Value_Is_Valid()
    {
        var code = ShortCode.Create("abc123");

        code.Value.Should().Be("abc123");
    }

    [Theory]
    [InlineData("")]
    [InlineData("ab")]
    [InlineData("abcdefghijklmnop")]
    public void Create_Should_Throw_When_Value_Is_Invalid(string code)
    {
        var action = () => ShortCode.Create(code);

        action.Should()
              .Throw<DomainException>();
    }
}