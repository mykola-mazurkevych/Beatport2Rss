#pragma warning disable CA1822 // Mark members as static

using System.Net;

namespace Beatport2Rss.WebApi.Responses;

internal readonly record struct InternalServerErrorResponse
{
    public Uri Type => new("https://datatracker.ietf.org/doc/html/rfc9110#section-15.6.1");
    public required string Title { get; init; }
    public HttpStatusCode Status => HttpStatusCode.InternalServerError;
    public required string? Detail { get; init; }
}