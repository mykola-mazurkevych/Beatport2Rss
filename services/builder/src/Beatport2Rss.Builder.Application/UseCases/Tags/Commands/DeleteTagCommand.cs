using Beatport2Rss.Builder.Application.Interfaces.Persistence.Repositories;
using Beatport2Rss.Common.Domain.Tags;
using Beatport2Rss.Common.EntityFrameworkCore.Persistence.Interfaces;

namespace Beatport2Rss.Builder.Application.UseCases.Tags.Commands;

public sealed record DeleteTagCommand(Guid TagId);

public sealed class DeleteTagCommandHandler(
    ITagCommandRepository tagCommandRepository,
    IUnitOfWork unitOfWork)
{
    public async Task HandleAsync(
        DeleteTagCommand command,
        CancellationToken cancellationToken)
    {
        var tagId = TagId.Create(command.TagId);
        var tag = await tagCommandRepository.FindAsync(tagId, cancellationToken);
        if (tag is null)
        {
            return;
        }

        tagCommandRepository.Delete(tag);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}