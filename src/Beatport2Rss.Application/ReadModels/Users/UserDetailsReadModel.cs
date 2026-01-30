using Beatport2Rss.Domain.Users;

namespace Beatport2Rss.Application.ReadModels.Users;

public sealed record UserDetailsReadModel(
    EmailAddress EmailAddress,
    string? FirstName,
    string? LastName,
    bool IsActive,
    int FeedsCount,
    int TagsCount);