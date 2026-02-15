namespace Beatport2Rss.WebApi.Endpoints.Sessions.Requests;

public sealed record UpdateSessionRequest(
    string? RefreshToken);