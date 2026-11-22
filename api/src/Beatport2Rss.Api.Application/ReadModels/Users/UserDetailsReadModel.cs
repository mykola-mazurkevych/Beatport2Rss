using Beatport2Rss.Common.Domain.Common;

namespace Beatport2Rss.Api.Application.ReadModels.Users;

public sealed record UserDetailsReadModel(
    EmailAddress EmailAddress,
    string? FirstName,
    string? LastName,
    bool IsActive,
    int FeedsCount,
    int TagsCount);