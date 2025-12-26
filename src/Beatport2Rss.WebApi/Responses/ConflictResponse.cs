using System.Net;

namespace Beatport2Rss.WebApi.Responses;

internal sealed record ConflictResponse
{
    ////public required Uri Type { get; init; }
    public required string Title { get; init; }
    public required HttpStatusCode Status { get; init; }
    public required string? Detail { get; init; }
}