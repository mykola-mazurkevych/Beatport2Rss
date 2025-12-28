namespace Beatport2Rss.WebApi.Responses;

internal readonly record struct ForbiddenResponse
{
    ////public required Uri Type { get; init; }
    public required string Error { get; init; }
}