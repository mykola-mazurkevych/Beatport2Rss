using Beatport2Rss.Application.Dtos.Tags;
using Beatport2Rss.Application.Extensions;
using Beatport2Rss.Application.Interfaces.Messages;
using Beatport2Rss.Application.Interfaces.Persistence;
using Beatport2Rss.Application.Interfaces.Persistence.Repositories;
using Beatport2Rss.Application.Interfaces.Services.Misc;
using Beatport2Rss.Domain.Tags;
using Beatport2Rss.Domain.Users;

using FluentResults;

using FluentValidation;

using Mediator;

namespace Beatport2Rss.Application.UseCases.Tags.Commands;

public sealed record CreateTagCommand(
    UserId UserId,
    string? Name) :
    ICommand<Result<TagDto>>, IRequireActiveUser;

internal sealed class CreateTagCommandValidator :
    AbstractValidator<CreateTagCommand>
{
    public CreateTagCommandValidator()
    {
        RuleFor(c => c.Name)
            .IsTagName();
    }
}

internal sealed class CreateTagCommandHandler(
    IClock clock,
    ISlugGenerator slugGenerator,
    ITagCommandRepository tagCommandRepository,
    IUnitOfWork unitOfWork) :
    ICommandHandler<CreateTagCommand, Result<TagDto>>
{
    public async ValueTask<Result<TagDto>> Handle(
        CreateTagCommand command,
        CancellationToken cancellationToken)
    {
        var tagName = TagName.Create(command.Name);
        var slug = slugGenerator.Generate(tagName.Value);

        if (await tagCommandRepository.ExistsAsync(t => t.UserId == command.UserId && t.Name == tagName, cancellationToken))
        {
            return Result.Conflict($"Tag name '{tagName}' is already taken.");
        }

        var tag = Tag.Create(
            clock.UtcNow,
            command.UserId,
            tagName,
            slug);

        await tagCommandRepository.AddAsync(tag, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new TagDto(
            tag.Id,
            tag.Name,
            tag.Slug,
            tag.CreatedAt);
    }
}