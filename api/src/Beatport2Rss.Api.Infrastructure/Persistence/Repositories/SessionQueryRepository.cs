using Beatport2Rss.Api.Application.Interfaces.Persistence.Repositories;
using Beatport2Rss.Api.Application.Interfaces.Services.Misc;
using Beatport2Rss.Api.Application.ReadModels.Sessions;
using Beatport2Rss.Api.Domain.Sessions;
using Beatport2Rss.Api.Domain.Users;
using Beatport2Rss.Api.Infrastructure.Persistence.QueryModels;

namespace Beatport2Rss.Api.Infrastructure.Persistence.Repositories;

internal sealed class SessionQueryRepository(
    IClock clock,
    IQueryable<SessionQueryModel> sessionQueryModels) :
    QueryRepository<SessionQueryModel, SessionId>(sessionQueryModels),
    ISessionQueryRepository
{
    public Task<bool> ExistsAsync(
        UserId userId,
        SessionId sessionId,
        CancellationToken cancellationToken = default) =>
        ExistsAsync(
            sessionQueryModel =>
                sessionQueryModel.UserId == userId &&
                sessionQueryModel.Id == sessionId,
            cancellationToken);

    public async Task<SessionDetailsReadModel> LoadSessionDetailsAsync(
        UserId userId,
        SessionId sessionId,
        CancellationToken cancellationToken = default) =>
        await LoadAsync(
            sessionQueryModel =>
                sessionQueryModel.UserId == userId &&
                sessionQueryModel.Id == sessionId,
            sessionQueryModel => new SessionDetailsReadModel(
                sessionQueryModel.Id,
                sessionQueryModel.CreatedAt,
                sessionQueryModel.EmailAddress,
                sessionQueryModel.FirstName,
                sessionQueryModel.LastName,
                sessionQueryModel.UserAgent,
                sessionQueryModel.IpAddress,
                sessionQueryModel.RefreshTokenExpiresAt < clock.UtcNow),
            cancellationToken);
}