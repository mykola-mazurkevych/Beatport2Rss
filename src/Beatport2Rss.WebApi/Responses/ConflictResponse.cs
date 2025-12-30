#pragma warning disable CA1822 // Mark members as static

using System.Net;

namespace Beatport2Rss.WebApi.Responses;

internal readonly record struct ConflictResponse
{
    public Uri Type => new("https://datatracker.ietf.org/doc/html/rfc9110#section-15.5.10");
    public required string Title { get; init; }
    public HttpStatusCode Status => HttpStatusCode.Conflict;
    public required string Detail { get; init; }
}