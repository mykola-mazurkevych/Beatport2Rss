using Beatport2Rss.Application.Extensions;
using Beatport2Rss.Application.Interfaces.Messages;
using Beatport2Rss.Application.Interfaces.Persistence;
using Beatport2Rss.Application.Interfaces.Persistence.Repositories;
using Beatport2Rss.Application.Interfaces.Services.Misc;
using Beatport2Rss.Domain.Common.ValueObjects;
using Beatport2Rss.Domain.Feeds;
using Beatport2Rss.Domain.Users;
using Beatport2Rss.SharedKernel.Extensions;

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
    IFeedCommandRepository feedCommandRepository,
    IUnitOfWork unitOfWork) :
    ICommandHandler<UpdateFeedCommand, Result<Slug>>
{
    public async ValueTask<Result<Slug>> Handle(
        UpdateFeedCommand command,
        CancellationToken cancellationToken)
    {
        var feed = await feedCommandRepository.LoadAsync(f => f.UserId == command.UserId && f.Slug == command.Slug, cancellationToken);

        var feedName = FeedName.Create(command.Name);
        var slug = command.UpdateSlug ? slugGenerator.Generate(feedName.Value) : feed.Slug;

        if (await feedCommandRepository.ExistsAsync(f => f.UserId == command.UserId && f.Name == feedName && f.Id != feed.Id, cancellationToken))
        {
            return Result.Conflict($"Feed name '{feedName}' is already taken.");
        }

        feed.UpdateName(feedName);
        feed.UpdateSlug(slug);
        feed.UpdateStatus(command.IsActive);

        feedCommandRepository.Update(feed);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return feed.Slug;
    }
}