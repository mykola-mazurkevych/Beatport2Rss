using Beatport2Rss.Domain.Users;

namespace Beatport2Rss.Application.ReadModels.Sessions;

public sealed record SessionDetailsReadModel(
    Guid SessionId,
    DateTimeOffset CreatedAt,
    EmailAddress EmailAddress,
    string? FirstName,
    string? LastName,
    string? UserAgent,
    string? IpAddress,
    bool IsExpired);