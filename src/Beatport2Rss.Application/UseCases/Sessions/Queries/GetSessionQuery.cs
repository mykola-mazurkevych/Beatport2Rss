using Beatport2Rss.Application.Extensions;
using Beatport2Rss.Application.Interfaces.Persistence.Repositories;
using Beatport2Rss.Domain.Sessions;
using Beatport2Rss.Domain.Users;

using FluentResults;

using FluentValidation;

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
    Guid UserId,
    Guid SessionId) :
    IQuery<Result<GetSessionResponse>>;

internal sealed class GetSessionQueryValidator :
    AbstractValidator<GetSessionQuery>
{
    public GetSessionQueryValidator()
    {
        RuleFor(q => q.UserId).IsUserId();
        RuleFor(q => q.SessionId).IsSessionId();
    }
}

internal sealed class GetSessionQueryHandler(
    ISessionQueryRepository sessionQueryRepository) :
    IQueryHandler<GetSessionQuery, Result<GetSessionResponse>>
{
    public async ValueTask<Result<GetSessionResponse>> Handle(
        GetSessionQuery query,
        CancellationToken cancellationToken = default)
    {
        var userId = UserId.Create(query.UserId);
        var sessionId = SessionId.Create(query.SessionId);

        var readModel = await sessionQueryRepository.LoadSessionDetailsQueryModelAsync(userId, sessionId, cancellationToken);

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