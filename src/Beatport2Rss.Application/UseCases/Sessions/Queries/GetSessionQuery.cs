#pragma warning disable CA1822 // Mark members as static

using Beatport2Rss.Application.Extensions;
using Beatport2Rss.Application.Interfaces.Persistence.Repositories;
using Beatport2Rss.Application.Types;
using Beatport2Rss.Domain.Sessions;
using Beatport2Rss.Domain.Users;

using FluentValidation;

using OneOf;

namespace Beatport2Rss.Application.UseCases.Sessions.Queries;

public readonly record struct GetSessionQuery(
    Guid UserId,
    Guid SessionId);

public sealed class GetSessionQueryValidator : AbstractValidator<GetSessionQuery>
{
    public GetSessionQueryValidator()
    {
        RuleFor(c => c.UserId)
            .NotEmpty().WithMessage("User ID is required.");
        
        RuleFor(c => c.SessionId)
            .NotEmpty().WithMessage("Session ID is required.");
    }
}

public readonly record struct GetSessionResult(
    Guid SessionId,
    DateTimeOffset CreatedAt,
    string EmailAddress,
    string? FirstName,
    string? LastName,
    string? UserAgent,
    string? IpAddress);

public sealed class GetSessionQueryHandler(
    IValidator<GetSessionQuery> validator,
    ISessionQueryRepository sessionQueryRepository,
    IUserQueryRepository userQueryRepository)
{
    public async Task<OneOf<Success<GetSessionResult>, ValidationFailed, NotFound, Unprocessable>> HandleAsync(
        GetSessionQuery query,
        CancellationToken cancellationToken = default)
    {
        var validationResult = await validator.ValidateAsync(query, cancellationToken);
        if (!validationResult.IsValid)
        {
            return new ValidationFailed(validationResult.GetErrors());
        }

        var userId = UserId.Create(query.UserId);
        var user = await userQueryRepository.GetAsync(userId, cancellationToken);
        if (user is null)
        {
            return new Unprocessable("User not found.");
        }

        var sessionId = SessionId.Create(query.SessionId);
        var session = await sessionQueryRepository.GetAsync(sessionId, cancellationToken);
        if (session is null)
        {
            return new NotFound("Session not found.");
        }

        var result = new GetSessionResult(session.Id,
            session.CreatedAt,
            user.EmailAddress,
            user.FirstName,
            user.LastName,
            session.UserAgent,
            session.IpAddress);

        return new Success<GetSessionResult>(result);
    }
}