using Beatport2Rss.Domain.Users;
using Beatport2Rss.SharedKernel.Common;

namespace Beatport2Rss.Infrastructure.QueryModels.Users;

internal sealed record UserQueryModel(
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
    IQueryModel<UserId>;