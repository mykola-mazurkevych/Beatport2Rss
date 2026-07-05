using Beatport2Rss.Api.Domain.Users;

namespace Beatport2Rss.Api.Application.ReadModels.Users;

public sealed record UserDetailsReadModel(
    EmailAddress EmailAddress,
    string? FirstName,
    string? LastName,
    bool IsActive,
    int FeedsCount,
    int TagsCount);