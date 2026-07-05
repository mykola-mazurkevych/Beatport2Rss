using Beatport2Rss.Api.Domain.Users;

namespace Beatport2Rss.Api.Application.Dtos.Users;

public sealed record UserDto(
    EmailAddress EmailAddress,
    string? FirstName,
    string? LastName,
    bool IsActive,
    int FeedsCount,
    int TagsCount);