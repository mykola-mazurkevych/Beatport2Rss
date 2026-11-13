using Beatport2Rss.Builder.Application.Interfaces.Persistence.Repositories;
using Beatport2Rss.Builder.Domain.Tags;
using Beatport2Rss.Common.Domain.Tags;
using Beatport2Rss.Common.EntityFrameworkCore.Persistence.Interfaces;
using Beatport2Rss.Common.Validation.Extensions;

using FluentValidation;

namespace Beatport2Rss.Builder.Application.UseCases.Tags.Commands;

public sealed record CreateTagCommand(
    Guid TagId,
    DateTimeOffset CreatedAt,
    string Name);

public sealed class CreateTagCommandValidator :
    AbstractValidator<CreateTagCommand>
{
    public CreateTagCommandValidator()
    {
        RuleFor(command => command.TagId).NotEmpty();
        RuleFor(command => command.Name).IsTagName();
    }
}

public static class CreateTagCommandHandler
{
    public static async Task Handle(
        CreateTagCommand command,
        ITagCommandRepository tagCommandRepository,
        IUnitOfWork unitOfWork,
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
