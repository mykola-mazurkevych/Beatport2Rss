using Beatport2Rss.Application.Extensions;
using Beatport2Rss.Application.Interfaces.Messages;
using Beatport2Rss.Application.Interfaces.Persistence;
using Beatport2Rss.Application.Interfaces.Persistence.Repositories;
using Beatport2Rss.Application.Interfaces.Services.Misc;
using Beatport2Rss.Domain.Common.ValueObjects;
using Beatport2Rss.Domain.Tags;
using Beatport2Rss.Domain.Users;

using FluentResults;

using FluentValidation;

using Mediator;

namespace Beatport2Rss.Application.UseCases.Tags.Commands;

public sealed record UpdateTagNameCommand(
    UserId UserId,
    Slug Slug,
    string? Name) :
    ICommand<Result<Slug>>, IRequireActiveUser, IRequireTag;

internal sealed class UpdateTagNameCommandValidator :
    AbstractValidator<UpdateTagNameCommand>
{
    public UpdateTagNameCommandValidator()
    {
        RuleFor(c => c.Name)
            .IsTagName();
    }
}

internal sealed class UpdateTagNameCommandHandler(
    ISlugGenerator slugGenerator,
    ITagCommandRepository tagCommandRepository,
    IUnitOfWork unitOfWork) :
    ICommandHandler<UpdateTagNameCommand, Result<Slug>>
{
    public async ValueTask<Result<Slug>> Handle(
        UpdateTagNameCommand command,
        CancellationToken cancellationToken)
    {
        var tag = await tagCommandRepository.LoadAsync(t => t.UserId == command.UserId && t.Slug == command.Slug, cancellationToken);

        var tagName = TagName.Create(command.Name);
        var slug = slugGenerator.Generate(tagName.Value);

        if (await tagCommandRepository.ExistsAsync(t => t.UserId == command.UserId && t.Name == tagName && t.Id != tag.Id, cancellationToken))
        {
            return Result.Conflict($"Tag name '{tagName}' is already taken.");
        }

        tag.UpdateName(tagName);
        tag.UpdateSlug(slug);

        tagCommandRepository.Update(tag);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return tag.Slug;
    }
}