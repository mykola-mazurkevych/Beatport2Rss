using Beatport2Rss.Api.Domain.Users;
using Beatport2Rss.Common.SharedKernel.Interfaces;

namespace Beatport2Rss.Api.Infrastructure.Persistence.QueryModels;

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