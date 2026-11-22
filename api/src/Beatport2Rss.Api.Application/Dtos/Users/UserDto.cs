using Beatport2Rss.Common.Domain.Common;

namespace Beatport2Rss.Api.Application.Dtos.Users;

public sealed record UserDto(
    EmailAddress EmailAddress,
    string? FirstName,
    string? LastName,
    bool IsActive,
    int FeedsCount,
    int TagsCount);