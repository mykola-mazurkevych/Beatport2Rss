using Beatport2Rss.Collector.Application.Interfaces.Persistence.Repositories;
using Beatport2Rss.Collector.Domain.Users;
using Beatport2Rss.Common.Domain.Users;
using Beatport2Rss.Common.EntityFrameworkCore.Persistence.Interfaces;

namespace Beatport2Rss.Collector.Application.UseCases.Users.Commands;

public sealed record CreateUserCommand(Guid UserId, DateTimeOffset CreatedAt);

public sealed class CreateUserCommandHandler(
    IUserCommandRepository userCommandRepository,
    IUnitOfWork unitOfWork)
{
    public async Task HandleAsync(CreateUserCommand command, CancellationToken cancellationToken)
    {
        var userId = UserId.Create(command.UserId);
        if (await userCommandRepository.ExistsAsync(userId, cancellationToken))
        {
            return;
        }

        var user = User.Create(
            userId,
            command.CreatedAt);
        await userCommandRepository.AddAsync(user, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}