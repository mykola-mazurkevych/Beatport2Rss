#pragma warning disable CA1822 // Mark members as static

using Beatport2Rss.Application.UseCases.Sessions.Interfaces;
using Beatport2Rss.Application.UseCases.Users.Interfaces;
using Beatport2Rss.Domain.Sessions;
using Beatport2Rss.Domain.Users;

using FluentValidation;

namespace Beatport2Rss.Application.UseCases.Sessions.Queries;

public readonly record struct GetSessionQuery(Guid SessionId, Guid UserId) :
    INeedSessionRequest,
    INeedUserRequest;

public sealed class GetSessionQueryValidator : AbstractValidator<GetSessionQuery>
{
    public GetSessionQueryValidator()
    {
        RuleFor(c => c.SessionId)
            .NotEmpty().WithMessage("Session ID is required.");

        RuleFor(c => c.UserId)
            .NotEmpty().WithMessage("User ID is required.");
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

public sealed class GetSessionQueryHandler
{
    public GetSessionResult Handle(GetSessionQuery query, Session session, User user) =>
        new(session.Id,
            session.CreatedAt,
            user.EmailAddress,
            user.FirstName,
            user.LastName,
            session.UserAgent,
            session.IpAddress);
}