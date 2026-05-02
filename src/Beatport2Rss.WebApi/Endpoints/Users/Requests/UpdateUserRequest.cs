namespace Beatport2Rss.WebApi.Endpoints.Users.Requests;

internal sealed record UpdateUserRequest(
    string? FirstName,
    string? LastName,
    string? CountryCode);