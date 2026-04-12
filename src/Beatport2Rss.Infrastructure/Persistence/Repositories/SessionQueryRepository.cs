using System.Linq.Expressions;

using Beatport2Rss.Application.Interfaces.Models.Sessions;
using Beatport2Rss.Application.Interfaces.Persistence.Repositories;
using Beatport2Rss.Application.Interfaces.Services.Misc;
using Beatport2Rss.Domain.Sessions;
using Beatport2Rss.Domain.Users;
using Beatport2Rss.Infrastructure.Persistence.QueryModels;

namespace Beatport2Rss.Infrastructure.Persistence.Repositories;

internal sealed class SessionQueryRepository(
    IClock clock,
    IQueryable<Session> sessions,
    IQueryable<SessionQueryModel> sessionQueryModels) :
    QueryRepository<SessionQueryModel, SessionId>(sessionQueryModels),
    ISessionQueryRepository
{
    public IQueryable<Session> Sessions => sessions;

    public Task<bool> ExistsAsync(
        UserId userId,
        SessionId sessionId,
        CancellationToken cancellationToken = default) =>
        ExistsAsync(
            sessionQueryModel =>
                sessionQueryModel.UserId == userId &&
                sessionQueryModel.Id == sessionId,
            cancellationToken);

    public async Task<IHaveSessionDetails> LoadSessionDetailsAsync(
        UserId userId,
        SessionId sessionId,
        CancellationToken cancellationToken = default) =>
        await LoadAsync(
            sessionQueryModel =>
                sessionQueryModel.UserId == userId &&
                sessionQueryModel.Id == sessionId,
            SessionDetails.CreateSelector(clock.UtcNow),
            cancellationToken);

    private sealed record SessionDetails(
        SessionId Id,
        DateTimeOffset CreatedAt,
        EmailAddress EmailAddress,
        string? FirstName,
        string? LastName,
        string? UserAgent,
        string? IpAddress,
        bool IsExpired) :
        IHaveSessionDetails
    {
        public static Expression<Func<SessionQueryModel, SessionDetails>> CreateSelector(DateTimeOffset now) =>
            sessionQueryModel => new SessionDetails(
                sessionQueryModel.Id,
                sessionQueryModel.CreatedAt,
                sessionQueryModel.EmailAddress,
                sessionQueryModel.FirstName,
                sessionQueryModel.LastName,
                sessionQueryModel.UserAgent,
                sessionQueryModel.IpAddress,
                sessionQueryModel.RefreshTokenExpiresAt < now);
    }
}