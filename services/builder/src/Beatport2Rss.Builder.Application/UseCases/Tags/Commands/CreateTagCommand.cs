using Beatport2Rss.Builder.Application.Interfaces.Persistence.Repositories;
using Beatport2Rss.Builder.Domain.Tags;
using Beatport2Rss.Common.Domain.Tags;
using Beatport2Rss.Common.EntityFrameworkCore.Persistence.Interfaces;

namespace Beatport2Rss.Builder.Application.UseCases.Tags.Commands;

public sealed record CreateTagCommand(
    Guid TagId,
    DateTimeOffset CreatedAt,
    string Name);

public sealed class CreateTagCommandHandler(
    ITagCommandRepository tagCommandRepository,
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

        var tag = Tag.Create(
            tagId,
            command.CreatedAt,
            TagName.Create(command.Name));

        await tagCommandRepository.AddAsync(tag, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}