using Beatport2Rss.Api.Application.Extensions;
using Beatport2Rss.Api.Application.Interfaces.Messages;
using Beatport2Rss.Api.Application.Interfaces.Persistence.Repositories;
using Beatport2Rss.Api.Application.Interfaces.Services.Misc;
using Beatport2Rss.Api.Domain.Feeds;
using Beatport2Rss.Api.Domain.Users;
using Beatport2Rss.Common.EntityFrameworkCore.Interfaces;
using Beatport2Rss.Common.SharedKernel.Extensions;
using Beatport2Rss.Common.SharedKernel.ValueObjects;

using FluentResults;

using FluentValidation;

using Mediator;

namespace Beatport2Rss.Api.Application.UseCases.Feeds.Commands;

public sealed record UpdateFeedCommand(
    UserId UserId,
    Slug FeedSlug,
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
        var feed = await feedCommandRepository.LoadAsync(command.UserId, command.FeedSlug, cancellationToken);

        var feedName = FeedName.Create(command.Name);
        var slug = command.UpdateSlug
            ? slugGenerator.Generate(feedName.Value)
            : feed.Slug;

        if (await feedCommandRepository.ExistsExceptAsync(command.UserId, feedName, feed.Id, cancellationToken))
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