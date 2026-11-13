using Beatport2Rss.Builder.Application.Interfaces.Persistence.Repositories;
using Beatport2Rss.Common.Domain.Tags;
using Beatport2Rss.Common.EntityFrameworkCore.Persistence.Interfaces;
using Beatport2Rss.Common.Validation.Extensions;

using FluentValidation;

namespace Beatport2Rss.Builder.Application.UseCases.Tags.Commands;

public sealed record UpdateTagCommand(Guid TagId, string Name);

public sealed class UpdateTagCommandValidator :
    AbstractValidator<UpdateTagCommand>
{
    public UpdateTagCommandValidator()
    {
        RuleFor(command => command.TagId).NotEmpty();
        RuleFor(command => command.Name).IsTagName();
    }
}

public static class UpdateTagCommandHandler
{
    public static async Task Handle(
        UpdateTagCommand command,
        ITagCommandRepository tagCommandRepository,
        IUnitOfWork unitOfWork,
        CancellationToken cancellationToken)
    {
        var tag = await tagCommandRepository.FindAsync(TagId.Create(command.TagId), cancellationToken);
        if (tag is null)
        {
            return;
        }

        tag.UpdateName(TagName.Create(command.Name));
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
