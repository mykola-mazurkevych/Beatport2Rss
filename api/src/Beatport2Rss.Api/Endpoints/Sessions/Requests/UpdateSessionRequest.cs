namespace Beatport2Rss.Api.Endpoints.Sessions.Requests;

internal sealed record UpdateSessionRequest(
    string? RefreshToken);