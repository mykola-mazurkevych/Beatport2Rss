namespace Beatport2Rss.WebApi.Requests.Users;

internal readonly record struct CreateUserRequestBody(
    string? EmailAddress,
    string? Password,
    string? FirstName,
    string? LastName);