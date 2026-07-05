using Beatport2Rss.Api.Domain.Sessions;
using Beatport2Rss.Api.Domain.Users;
using Beatport2Rss.Common.SharedKernel.Interfaces;

namespace Beatport2Rss.Api.Infrastructure.Persistence.QueryModels;

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