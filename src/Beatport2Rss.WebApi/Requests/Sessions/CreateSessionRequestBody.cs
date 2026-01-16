namespace Beatport2Rss.WebApi.Requests.Sessions;

internal readonly record struct CreateSessionRequestBody(
    string? EmailAddress,
    string? Password);