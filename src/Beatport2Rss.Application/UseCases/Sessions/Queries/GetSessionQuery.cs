using Beatport2Rss.Application.Interfaces.Persistence.Repositories;
using Beatport2Rss.Domain.Sessions;
using Beatport2Rss.Domain.Users;

using FluentResults;

using Mediator;

namespace Beatport2Rss.Application.UseCases.Sessions.Queries;

public sealed record GetSessionResponse(
    SessionId SessionId,
    DateTimeOffset CreatedAt,
    EmailAddress EmailAddress,
    string? FirstName,
    string? LastName,
    string? UserAgent,
    string? IpAddress,
    bool IsExpired);

public sealed record GetSessionQuery(
    UserId UserId,
    SessionId SessionId) :
    IQuery<Result<GetSessionResponse>>;

internal sealed class GetSessionQueryHandler(
    ISessionQueryRepository sessionQueryRepository) :
    IQueryHandler<GetSessionQuery, Result<GetSessionResponse>>
{
    public async ValueTask<Result<GetSessionResponse>> Handle(
        GetSessionQuery query,
        CancellationToken cancellationToken = default)
    {
        var sessionDetails = await sessionQueryRepository.LoadSessionDetailsAsync(query.UserId, query.SessionId, cancellationToken);

        return new GetSessionResponse(
            sessionDetails.Id,
            sessionDetails.CreatedAt,
            sessionDetails.EmailAddress,
            sessionDetails.FirstName,
            sessionDetails.LastName,
            sessionDetails.UserAgent,
            sessionDetails.IpAddress,
            sessionDetails.IsExpired);
    }
}