using System.Security.Cryptography;
using UrlShortener.Application.Abstractions.Services;

namespace UrlShortener.Infrastructure.Services;

public sealed class Base62ShortCodeGenerator : IShortCodeGenerator
{
    private const string Alphabet =
        "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";

    public string Generate()
    {
        Span<byte> bytes = stackalloc byte[8];
        RandomNumberGenerator.Fill(bytes);

        ulong value = BitConverter.ToUInt64(bytes);

        return EncodeBase62(value);
    }

    private static string EncodeBase62(ulong value)
    {
        if (value == 0) return "0";

        var result = new System.Text.StringBuilder();

        while (value > 0)
        {
            result.Insert(0, Alphabet[(int)(value % 62)]);
            value /= 62;
        }

        return result.ToString();
    }
}