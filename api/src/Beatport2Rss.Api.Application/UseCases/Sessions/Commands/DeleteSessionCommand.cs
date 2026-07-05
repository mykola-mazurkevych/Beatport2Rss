using Beatport2Rss.Api.Application.Interfaces.Persistence;
using Beatport2Rss.Api.Application.Interfaces.Persistence.Repositories;
using Beatport2Rss.Api.Domain.Sessions;
using Beatport2Rss.Common.SharedKernel.Extensions;

using FluentResults;

using Mediator;

namespace Beatport2Rss.Api.Application.UseCases.Sessions.Commands;

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