using Beatport2Rss.Application.Interfaces.Persistence.Repositories;
using Beatport2Rss.Application.Interfaces.Services.Misc;

using FluentResults;

using Mediator;

namespace Beatport2Rss.Application.UseCases.Sessions.Commands;

public sealed record DeleteExpiredSessionsCommand :
    ICommand<Result>;

internal sealed class DeleteExpiredSessionsCommandHandler(
    IClock clock,
    ISessionCommandRepository sessionCommandRepository) :
    ICommandHandler<DeleteExpiredSessionsCommand, Result>
{
    public async ValueTask<Result> Handle(
        DeleteExpiredSessionsCommand command,
        CancellationToken cancellationToken = default)
    {
        await sessionCommandRepository.ExecuteDeleteAsync(s => s.RefreshTokenExpiresAt < clock.UtcNow, cancellationToken);

        return Result.Ok();
    }
}