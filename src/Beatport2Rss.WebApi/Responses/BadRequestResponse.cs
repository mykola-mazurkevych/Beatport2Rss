using System.Net;

namespace Beatport2Rss.WebApi.Responses;

internal sealed record BadRequestResponse
{
    ////public required Uri Type { get; init; }
    public required string Title { get; init; }
    public required HttpStatusCode Status { get; init; }
    public required string Detail { get; init; }

    public required IReadOnlyDictionary<string, IEnumerable<string>> Errors { get; init; }
}