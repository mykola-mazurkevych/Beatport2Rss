namespace Beatport2Rss.WebApi.Endpoints.Users.Requests;

internal sealed record CreateUserRequest(
    string? EmailAddress,
    string? Password,
    string? FirstName,
    string? LastName);