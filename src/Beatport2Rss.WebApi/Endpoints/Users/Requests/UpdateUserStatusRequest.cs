namespace Beatport2Rss.WebApi.Endpoints.Users.Requests;

internal sealed record UpdateUserStatusRequest(
    bool IsActive);