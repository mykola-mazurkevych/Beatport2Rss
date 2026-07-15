using Beatport2Rss.Api.Application.Dtos.Tags;
using Beatport2Rss.Api.Application.Extensions;
using Beatport2Rss.Api.Application.Interfaces.Messages;
using Beatport2Rss.Api.Application.Interfaces.Persistence.Repositories;
using Beatport2Rss.Api.Application.Interfaces.Services.Misc;
using Beatport2Rss.Api.Domain.Tags;
using Beatport2Rss.Api.Domain.Users;
using Beatport2Rss.Common.EntityFrameworkCore.Interfaces;
using Beatport2Rss.Common.SharedKernel.Extensions;

using FluentResults;

using FluentValidation;

using Mediator;

namespace Beatport2Rss.Api.Application.UseCases.Tags.Commands;

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

        if (await tagCommandRepository.ExistsAsync(command.UserId, tagName, cancellationToken))
        {
            return Result.Conflict($"Tag name '{tagName}' is already taken.");
        }

        var tagId = TagId.Create(Guid.NewGuid());
        var tag = Tag.Create(
            tagId,
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