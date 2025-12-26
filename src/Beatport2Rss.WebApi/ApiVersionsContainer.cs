using Asp.Versioning;

namespace Beatport2Rss.WebApi;

internal static class ApiVersionsContainer
{
    public static ApiVersion V1 => new(1);

    public static ApiVersion Default => V1;
}