namespace Beatport2Rss.WebApi.Endpoints.Users.Requests;

public sealed record UpdateUserStatusRequest(
    bool IsActive);