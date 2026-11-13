using Beatport2Rss.Builder.Application.Interfaces.Persistence.Repositories;
using Beatport2Rss.Common.Domain.Tags;
using Beatport2Rss.Common.EntityFrameworkCore.Persistence.Interfaces;

using FluentValidation;

namespace Beatport2Rss.Builder.Application.UseCases.Tags.Commands;

public sealed record DeleteTagCommand(Guid TagId);

public sealed class DeleteTagCommandValidator :
    AbstractValidator<DeleteTagCommand>
{
    public DeleteTagCommandValidator()
    {
        RuleFor(command => command.TagId).NotEmpty();
    }
}

public static class DeleteTagCommandHandler
{
    public static async Task Handle(
        DeleteTagCommand command,
        ITagCommandRepository tagCommandRepository,
        IUnitOfWork unitOfWork,
        CancellationToken cancellationToken)
    {
        var tag = await tagCommandRepository.FindAsync(TagId.Create(command.TagId), cancellationToken);
        if (tag is null)
        {
            return;
        }

        tagCommandRepository.Delete(tag);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
