namespace Beatport2Rss.WebApi.Endpoints.Sessions.Requests;

internal sealed record CreateSessionRequest(
    string? EmailAddress,
    string? Password);