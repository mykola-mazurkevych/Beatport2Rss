using Beatport2Rss.Builder.Application.Interfaces.Persistence.Repositories;
using Beatport2Rss.Builder.Domain.Tags;
using Beatport2Rss.Common.Domain.Tags;
using Beatport2Rss.Common.Domain.Users;
using Beatport2Rss.Common.EntityFrameworkCore.Persistence.Interfaces;
using Beatport2Rss.Common.Miscellaneous.Exceptions;

namespace Beatport2Rss.Builder.Application.UseCases.Tags.Commands;

public sealed record CreateTagCommand(
    Guid TagId,
    Guid UserId,
    DateTimeOffset CreatedAt,
    string Name);

public sealed class CreateTagCommandHandler(
    ITagCommandRepository tagCommandRepository,
    IUserCommandRepository userCommandRepository,
    IUnitOfWork unitOfWork)
{
    public async Task HandleAsync(
        CreateTagCommand command,
        CancellationToken cancellationToken)
    {
        var tagId = TagId.Create(command.TagId);
        if (await tagCommandRepository.ExistsAsync(tagId, cancellationToken))
        {
            return;
        }

        var userId = UserId.Create(command.UserId);
        if (!await userCommandRepository.ExistsAsync(userId, cancellationToken))
        {
            throw new RetryableException($"User {command.UserId} is not available yet.");
        }

        var tag = Tag.Create(
            tagId,
            command.CreatedAt,
            userId,
            TagName.Create(command.Name));

        await tagCommandRepository.AddAsync(tag, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}