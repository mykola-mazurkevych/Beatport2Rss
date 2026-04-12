using Beatport2Rss.Domain.Sessions;
using Beatport2Rss.Domain.Users;
using Beatport2Rss.SharedKernel.Common;

namespace Beatport2Rss.Infrastructure.Persistence.QueryModels;

internal sealed record SessionQueryModel(
    SessionId Id,
    DateTimeOffset CreatedAt,
    UserId UserId,
    EmailAddress EmailAddress,
    string? FirstName,
    string? LastName,
    string? UserAgent,
    string? IpAddress,
    DateTimeOffset RefreshTokenExpiresAt) :
    IQueryModel<SessionId>;