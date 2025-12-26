using System.Net;

namespace Beatport2Rss.WebApi.Responses;

internal readonly record struct BadRequestResponse
{
    ////public required Uri Type { get; init; }
    public required string Title { get; init; }
    public required HttpStatusCode Status { get; init; }
    public required string Detail { get; init; }
    public required string TraceId { get; init; }

    public required IReadOnlyDictionary<string, IEnumerable<string>> Errors { get; init; }
}