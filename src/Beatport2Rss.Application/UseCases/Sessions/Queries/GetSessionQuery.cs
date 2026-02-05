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
        var readModel = await sessionQueryRepository.LoadSessionDetailsReadModelAsync(query.UserId, query.SessionId, cancellationToken);

        return new GetSessionResponse(
            readModel.SessionId,
            readModel.CreatedAt,
            readModel.EmailAddress,
            readModel.FirstName,
            readModel.LastName,
            readModel.UserAgent,
            readModel.IpAddress,
            readModel.IsExpired);
    }
}