using Beatport2Rss.Application.Extensions;
using Beatport2Rss.Application.Interfaces.Persistence;
using Beatport2Rss.Application.Interfaces.Persistence.Repositories;
using Beatport2Rss.Domain.Sessions;

using FluentResults;

using FluentValidation;

using Mediator;

namespace Beatport2Rss.Application.UseCases.Sessions.Commands;

public readonly record struct DeleteSessionCommand(Guid SessionId) :
    ICommand<Result>;

internal sealed class DeleteSessionCommandValidator :
    AbstractValidator<DeleteSessionCommand>
{
    public DeleteSessionCommandValidator()
    {
        RuleFor(c => c.SessionId)
            .NotEmpty().WithMessage("Session ID is required.");
    }
}

internal sealed class DeleteSessionCommandHandler(
    ISessionCommandRepository sessionRepository,
    IUnitOfWork unitOfWork) :
    ICommandHandler<DeleteSessionCommand, Result>
{
    public async ValueTask<Result> Handle(
        DeleteSessionCommand command,
        CancellationToken cancellationToken = default)
    {
        var sessionId = SessionId.Create(command.SessionId);
        var session = await sessionRepository.FindAsync(sessionId, cancellationToken);

        if (session is null)
        {
            return Result.NotFound("Session not found.");
        }

        sessionRepository.Delete(session);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok();
    }
}