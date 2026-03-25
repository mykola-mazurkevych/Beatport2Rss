namespace Beatport2Rss.WebApi.Endpoints;

internal sealed record PageNavigationRequest(
    int? PageSize,
    string? NextPage,
    string? PreviousPage);