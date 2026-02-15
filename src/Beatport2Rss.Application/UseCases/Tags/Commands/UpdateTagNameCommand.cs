using Beatport2Rss.Application.Dtos.Tags;
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
    ICommand<Result<TagDto>>, IRequireActiveUser, IRequireTag;

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
    IUserCommandRepository userCommandRepository,
    IUnitOfWork unitOfWork) :
    ICommandHandler<UpdateTagNameCommand, Result<TagDto>>
{
    public async ValueTask<Result<TagDto>> Handle(
        UpdateTagNameCommand command,
        CancellationToken cancellationToken)
    {
        var user = await userCommandRepository.LoadWithTagsAsync(command.UserId, cancellationToken);
        var tag = user.Tags.Single(t => t.Slug == command.Slug);

        var tagName = TagName.Create(command.Name);
        var slug = slugGenerator.Generate(tagName);

        if (user.Tags.Any(t => t.Name == command.Name && t.Id != tag.Id))
        {
            return Result.Conflict($"Tag name '{tagName}' is already taken.");
        }

        user.UpdateTag(tag.Id, tagName, slug);

        userCommandRepository.Update(user);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new TagDto(
            tag.Id,
            tag.Name,
            tag.Slug,
            tag.CreatedAt);
    }
}