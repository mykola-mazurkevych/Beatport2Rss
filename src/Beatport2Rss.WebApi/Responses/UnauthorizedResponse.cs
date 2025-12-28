namespace Beatport2Rss.WebApi.Responses;

internal readonly record struct UnauthorizedResponse
{
    ////public required Uri Type { get; init; }
    public required string Error { get; init; }
}