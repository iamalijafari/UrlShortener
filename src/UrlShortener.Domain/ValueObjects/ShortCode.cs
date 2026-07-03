using UrlShortener.Domain.Exceptions;

namespace UrlShortener.Domain.ValueObjects;

public sealed record ShortCode
{
    public string Value { get; }

    private ShortCode(string value)
    {
        Value = value;
    }

    public static ShortCode Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new DomainException("Short code cannot be empty.");

        if (value.Length < 5 || value.Length > 12)
            throw new DomainException("Short code length is invalid.");

        return new ShortCode(value);
    }

    public override string ToString()
    {
        return Value;
    }
}