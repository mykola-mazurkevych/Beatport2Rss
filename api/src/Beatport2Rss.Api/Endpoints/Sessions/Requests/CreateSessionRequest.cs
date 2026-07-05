namespace Beatport2Rss.Api.Endpoints.Sessions.Requests;

internal sealed record CreateSessionRequest(
    string? EmailAddress,
    string? Password);