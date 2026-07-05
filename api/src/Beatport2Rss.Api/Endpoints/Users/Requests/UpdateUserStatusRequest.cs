namespace Beatport2Rss.Api.Endpoints.Users.Requests;

internal sealed record UpdateUserStatusRequest(
    bool IsActive);