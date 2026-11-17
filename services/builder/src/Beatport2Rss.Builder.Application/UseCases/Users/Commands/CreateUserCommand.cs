using Beatport2Rss.Builder.Application.Interfaces.Persistence.Repositories;
using Beatport2Rss.Builder.Domain.Users;
using Beatport2Rss.Common.Domain.Users;
using Beatport2Rss.Common.EntityFrameworkCore.Persistence.Interfaces;

namespace Beatport2Rss.Builder.Application.UseCases.Users.Commands;

public sealed record CreateUserCommand(Guid UserId, DateTimeOffset CreatedAt);

public sealed class CreateUserCommandHandler(
    IUserCommandRepository userCommandRepository,
    IUnitOfWork unitOfWork)
{
    public async Task HandleAsync(CreateUserCommand command, CancellationToken cancellationToken)
    {
        var userId = UserId.Create(command.UserId);
        if (await userCommandRepository.FindAsync(userId, cancellationToken) is not null)
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