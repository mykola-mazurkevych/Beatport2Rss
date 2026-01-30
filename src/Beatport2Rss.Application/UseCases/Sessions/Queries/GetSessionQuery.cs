using Beatport2Rss.Application.Interfaces.Persistence.Repositories;
using Beatport2Rss.Application.ReadModels.Sessions;
using Beatport2Rss.Domain.Sessions;
using Beatport2Rss.Domain.Users;

using FluentResults;

using FluentValidation;

using Mediator;

namespace Beatport2Rss.Application.UseCases.Sessions.Queries;

public readonly record struct GetSessionQuery(
    Guid UserId,
    Guid SessionId) :
    IQuery<Result<SessionDetailsReadModel>>;

internal sealed class GetSessionQueryValidator :
    AbstractValidator<GetSessionQuery>
{
    public GetSessionQueryValidator()
    {
        RuleFor(q => q.UserId)
            .NotEmpty().WithMessage("User ID is required.");

        RuleFor(q => q.SessionId)
            .NotEmpty().WithMessage("Session ID is required.");
    }
}

internal sealed class GetSessionQueryHandler(
    ISessionQueryRepository sessionQueryRepository) :
    IQueryHandler<GetSessionQuery, Result<SessionDetailsReadModel>>
{
    public async ValueTask<Result<SessionDetailsReadModel>> Handle(
        GetSessionQuery query,
        CancellationToken cancellationToken = default)
    {
        var userId = UserId.Create(query.UserId);
        var sessionId = SessionId.Create(query.SessionId);

        var sessionDetailsQueryModel = await sessionQueryRepository.LoadSessionDetailsQueryModelAsync(userId, sessionId, cancellationToken);

        return sessionDetailsQueryModel;
    }
}