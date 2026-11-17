using Beatport2Rss.Collector.Application.Interfaces.Persistence.Repositories;
using Beatport2Rss.Common.Domain.Users;
using Beatport2Rss.Common.EntityFrameworkCore.Persistence.Interfaces;

namespace Beatport2Rss.Collector.Application.UseCases.Users.Commands;

public sealed record DeleteUserCommand(Guid UserId);

public sealed class DeleteUserCommandHandler(
    IUserCommandRepository userCommandRepository,
    IUnitOfWork unitOfWork)
{
    public async Task HandleAsync(DeleteUserCommand command, CancellationToken cancellationToken)
    {
        var userId = UserId.Create(command.UserId);
        var user = await userCommandRepository.FindAsync(userId, cancellationToken);
        if (user is null)
        {
            return;
        }

        userCommandRepository.Delete(user);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}