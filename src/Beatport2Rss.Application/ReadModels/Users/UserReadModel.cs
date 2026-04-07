using Beatport2Rss.Domain.Users;
using Beatport2Rss.SharedKernel.Common;

namespace Beatport2Rss.Application.ReadModels.Users;

public sealed record UserReadModel(
    UserId Id,
    DateTimeOffset CreatedAt,
    EmailAddress EmailAddress,
    PasswordHash PasswordHash,
    string? FirstName,
    string? LastName,
    string? FullName,
    UserStatus Status,
    bool IsActive,
    int FeedsCount,
    int TagsCount) :
    IReadModel<UserId>;