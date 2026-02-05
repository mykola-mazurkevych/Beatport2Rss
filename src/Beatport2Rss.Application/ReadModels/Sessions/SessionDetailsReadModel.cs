using Beatport2Rss.Domain.Sessions;
using Beatport2Rss.Domain.Users;

namespace Beatport2Rss.Application.ReadModels.Sessions;

public sealed record SessionDetailsReadModel(
    SessionId SessionId,
    DateTimeOffset CreatedAt,
    EmailAddress EmailAddress,
    string? FirstName,
    string? LastName,
    string? UserAgent,
    string? IpAddress,
    bool IsExpired);