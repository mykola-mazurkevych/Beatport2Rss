using Beatport2Rss.Application.Extensions;
using Beatport2Rss.Application.Interfaces.Messages;
using Beatport2Rss.Application.Interfaces.Persistence;
using Beatport2Rss.Application.Interfaces.Persistence.Repositories;
using Beatport2Rss.Application.Interfaces.Services.Misc;
using Beatport2Rss.Domain.Common.ValueObjects;
using Beatport2Rss.Domain.Feeds;
using Beatport2Rss.Domain.Users;

using FluentResults;

using FluentValidation;

using Mediator;

namespace Beatport2Rss.Application.UseCases.Feeds.Commands;

public sealed record UpdateFeedCommand(
    UserId UserId,
    Slug Slug,
    string? Name,
    bool UpdateSlug,
    bool IsActive) :
    ICommand<Result<Slug>>, IRequireUser, IRequireFeed;

internal sealed class UpdateFeedCommandValidator :
    AbstractValidator<UpdateFeedCommand>
{
    public UpdateFeedCommandValidator()
    {
        RuleFor(c => c.Name).IsFeedName();
    }
}

internal sealed class UpdateFeedCommandHandler(
    ISlugGenerator slugGenerator,
    IUserCommandRepository userCommandRepository,
    IUnitOfWork unitOfWork) :
    ICommandHandler<UpdateFeedCommand, Result<Slug>>
{
    public async ValueTask<Result<Slug>> Handle(
        UpdateFeedCommand command,
        CancellationToken cancellationToken)
    {
        var slug = command.Slug;
        var user = await userCommandRepository.LoadWithFeedsAsync(command.UserId, cancellationToken);

        var feedName = FeedName.Create(command.Name);
        user.UpdateFeedName(command.Slug, feedName);
        user.UpdateFeedStatus(command.Slug, command.IsActive);

        if (command.UpdateSlug)
        {
            slug = slugGenerator.Generate(feedName.Value);
            user.UpdateFeedSlug(command.Slug, slug);
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return slug;
    }
}