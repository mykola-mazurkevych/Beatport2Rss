namespace Beatport2Rss.WebApi.Requests.Sessions;

internal readonly record struct CreateSessionRequest(
    string? EmailAddress,
    string? Password);