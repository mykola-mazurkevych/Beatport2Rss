using Beatport2Rss.Application.Extensions;
using Beatport2Rss.Application.Interfaces.Persistence;
using Beatport2Rss.Application.Interfaces.Persistence.Repositories;
using Beatport2Rss.Domain.Sessions;

using FluentResults;

using Mediator;

namespace Beatport2Rss.Application.UseCases.Sessions.Commands;

public sealed record DeleteSessionCommand(
    SessionId SessionId) :
    ICommand<Result>;

internal sealed class DeleteSessionCommandHandler(
    ISessionCommandRepository sessionCommandRepository,
    IUnitOfWork unitOfWork) :
    ICommandHandler<DeleteSessionCommand, Result>
{
    public async ValueTask<Result> Handle(
        DeleteSessionCommand command,
        CancellationToken cancellationToken = default)
    {
        var session = await sessionCommandRepository.FindAsync(command.SessionId, cancellationToken);

        if (session is null)
        {
            return Result.NotFound("Session not found.");
        }

        sessionCommandRepository.Delete(session);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok();
    }
}