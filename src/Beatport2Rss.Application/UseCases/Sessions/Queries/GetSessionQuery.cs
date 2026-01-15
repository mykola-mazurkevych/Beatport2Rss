using Beatport2Rss.Application.Interfaces.Persistence.Repositories;
using Beatport2Rss.Domain.Sessions;

using FluentResults;

using FluentValidation;

using Mediator;

namespace Beatport2Rss.Application.UseCases.Sessions.Queries;

public readonly record struct GetSessionQuery(Guid SessionId) :
    IQuery<Result<GetSessionResult>>;

public readonly record struct GetSessionResult(
    Guid SessionId,
    DateTimeOffset CreatedAt,
    string EmailAddress,
    string? FirstName,
    string? LastName,
    string? UserAgent,
    string? IpAddress);

public sealed class GetSessionQueryValidator :
    AbstractValidator<GetSessionQuery>
{
    public GetSessionQueryValidator()
    {
        RuleFor(q => q.SessionId)
            .NotEmpty().WithMessage("Session ID is required.");
    }
}

public sealed class GetSessionQueryHandler(
    ISessionQueryRepository sessionRepository,
    IUserQueryRepository userRepository) :
    IQueryHandler<GetSessionQuery, Result<GetSessionResult>>
{
    public async ValueTask<Result<GetSessionResult>> Handle(
        GetSessionQuery query,
        CancellationToken cancellationToken = default)
    {
        var sessionId = SessionId.Create(query.SessionId);
        var session = await sessionRepository.LoadAsync(sessionId, cancellationToken);
        var user = await userRepository.LoadAsync(session.UserId, cancellationToken);

        var result = new GetSessionResult(
            session.Id,
            session.CreatedAt,
            user.EmailAddress,
            user.FirstName,
            user.LastName,
            session.UserAgent,
            session.IpAddress);

        return result;
    }
}