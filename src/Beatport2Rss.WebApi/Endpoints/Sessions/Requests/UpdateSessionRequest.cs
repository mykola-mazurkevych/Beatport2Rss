namespace Beatport2Rss.WebApi.Endpoints.Sessions.Requests;

internal sealed record UpdateSessionRequest(
    string? RefreshToken);