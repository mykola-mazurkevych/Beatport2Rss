using Beatport2Rss.Api.Domain.Sessions;
using Beatport2Rss.Api.Domain.Users;

namespace Beatport2Rss.Api.Application.ReadModels.Sessions;

public sealed record SessionDetailsReadModel(
    SessionId Id,
    DateTimeOffset CreatedAt,
    EmailAddress EmailAddress,
    string? FirstName,
    string? LastName,
    string? UserAgent,
    string? IpAddress,
    bool IsExpired);