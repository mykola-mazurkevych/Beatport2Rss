using Beatport2Rss.Domain.Users;

namespace Beatport2Rss.Application.Dtos.Users;

public sealed record UserDto(
    EmailAddress EmailAddress,
    string? FirstName,
    string? LastName,
    bool IsActive,
    int FeedsCount,
    int TagsCount);