using Beatport2Rss.Api.Application.Extensions;
using Beatport2Rss.Api.Application.Interfaces.Messages;
using Beatport2Rss.Api.Application.Interfaces.Persistence.Repositories;
using Beatport2Rss.Api.Application.Interfaces.Services.Misc;
using Beatport2Rss.Api.Domain.Tags;
using Beatport2Rss.Api.Domain.Users;
using Beatport2Rss.Common.EntityFrameworkCore.Interfaces;
using Beatport2Rss.Common.SharedKernel.Extensions;
using Beatport2Rss.Common.SharedKernel.ValueObjects;

using FluentResults;

using FluentValidation;

using Mediator;

namespace Beatport2Rss.Api.Application.UseCases.Tags.Commands;

public sealed record UpdateTagNameCommand(
    UserId UserId,
    Slug TagSlug,
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
        var tag = await tagCommandRepository.LoadAsync(command.UserId, command.TagSlug, cancellationToken);

        var tagName = TagName.Create(command.Name);
        var slug = slugGenerator.Generate(tagName.Value);

        if (await tagCommandRepository.ExistsExceptAsync(command.UserId, tagName, tag.Id, cancellationToken))
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