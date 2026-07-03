using UrlShortener.Domain.Exceptions;

namespace UrlShortener.Domain.ValueObjects;

public sealed record OriginalUrl
{
    public string Value { get; }

    private OriginalUrl(string value)
    {
        Value = value;
    }

    public static OriginalUrl Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new DomainException("Original URL cannot be empty.");

        if (!Uri.TryCreate(value, UriKind.Absolute, out var uri))
            throw new DomainException("Original URL is invalid.");

        if (uri.Scheme != Uri.UriSchemeHttp &&
            uri.Scheme != Uri.UriSchemeHttps)
            throw new DomainException("Only HTTP and HTTPS URLs are supported.");

        return new OriginalUrl(value.Trim());
    }

    public override string ToString()
    {
        return Value;
    }
}