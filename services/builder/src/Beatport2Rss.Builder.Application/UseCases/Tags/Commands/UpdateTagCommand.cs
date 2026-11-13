using Beatport2Rss.Builder.Application.Interfaces.Persistence.Repositories;
using Beatport2Rss.Common.Domain.Tags;
using Beatport2Rss.Common.EntityFrameworkCore.Persistence.Interfaces;

namespace Beatport2Rss.Builder.Application.UseCases.Tags.Commands;

public sealed record UpdateTagCommand(
    Guid TagId,
    string Name);

public sealed class UpdateTagCommandHandler(
    ITagCommandRepository tagCommandRepository,
    IUnitOfWork unitOfWork)
{
    public async Task HandleAsync(
        UpdateTagCommand command,
        CancellationToken cancellationToken)
    {
        var tagId = TagId.Create(command.TagId);
        var tag = await tagCommandRepository.FindAsync(tagId, cancellationToken);
        if (tag is null)
        {
            return;
        }

        tag.UpdateName(TagName.Create(command.Name));
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}