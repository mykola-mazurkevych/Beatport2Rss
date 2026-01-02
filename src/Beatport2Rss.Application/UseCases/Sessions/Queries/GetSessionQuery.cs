#pragma warning disable CA1822 // Mark members as static

using Beatport2Rss.Application.Extensions;
using Beatport2Rss.Application.Interfaces.Persistence.Repositories;
using Beatport2Rss.Application.Types;
using Beatport2Rss.Domain.Sessions;

using FluentValidation;

using OneOf;

namespace Beatport2Rss.Application.UseCases.Sessions.Queries;

public readonly record struct GetSessionQuery(Guid SessionId);

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
        RuleFor(c => c.SessionId)
            .NotEmpty().WithMessage("Session ID is required.");
    }
}

public sealed class GetSessionQueryHandler(
    IValidator<GetSessionQuery> validator,
    ISessionQueryRepository sessionQueryRepository,
    IUserQueryRepository userQueryRepository)
{
    public async Task<OneOf<Success<GetSessionResult>, ValidationFailed, InactiveUser>> HandleAsync(
        GetSessionQuery query,
        CancellationToken cancellationToken = default)
    {
        var validationResult = await validator.ValidateAsync(query, cancellationToken);
        if (!validationResult.IsValid)
        {
            return new ValidationFailed(validationResult.GetErrors());
        }

        var sessionId = SessionId.Create(query.SessionId);
        var session = (await sessionQueryRepository.GetAsync(sessionId, cancellationToken))!;
        var user = (await userQueryRepository.GetAsync(session.UserId, cancellationToken))!;

        if (!user.IsActive)
        {
            return new InactiveUser();
        }

        var result = new GetSessionResult(
            session.Id,
            session.CreatedAt,
            user.EmailAddress,
            user.FirstName,
            user.LastName,
            session.UserAgent,
            session.IpAddress);

        return new Success<GetSessionResult>(result);
    }
}