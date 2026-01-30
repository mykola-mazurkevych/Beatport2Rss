namespace Beatport2Rss.WebApi.Requests.Users;

internal readonly record struct UpdateUserStatusRequestBody(
    bool IsActive);