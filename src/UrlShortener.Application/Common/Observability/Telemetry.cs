using System.Diagnostics;

namespace UrlShortener.Application.Common.Observability;

public static class Telemetry
{
    public const string ServiceName = "UrlShortener";

    public static readonly ActivitySource ActivitySource = new(ServiceName);
}
