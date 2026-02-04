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

public sealed record CreateFeedRequest(
    string? Name);

public sealed record CreateFeedCommand(
    Guid UserId,
    string? Name) :
    ICommand<Result<Slug>>, IRequireActiveUser;

internal sealed class CreateFeedCommandValidator :
    AbstractValidator<CreateFeedCommand>
{
    public CreateFeedCommandValidator()
    {
        RuleFor(c => c.UserId).IsUserId();
        RuleFor(c => c.Name).IsFeedName();
    }
}

internal sealed class CreateFeedCommandHandler(
    IClock clock,
    ISlugGenerator slugGenerator,
    IUserCommandRepository userCommandRepository,
    IUnitOfWork unitOfWork) :
    ICommandHandler<CreateFeedCommand, Result<Slug>>
{
    public async ValueTask<Result<Slug>> Handle(
        CreateFeedCommand command,
        CancellationToken cancellationToken = default)
    {
        var userId = UserId.Create(command.UserId);
        var user = await userCommandRepository.LoadWithFeedsAsync(userId, cancellationToken);

        var feedId = FeedId.Create(Guid.CreateVersion7());
        var feedName = FeedName.Create(command.Name);
        var slug = slugGenerator.Generate(feedName);

        if (user.HasFeed(feedName))
        {
            return Result.Conflict($"Feed name '{feedName}' is already taken.");
        }

        var feed = Feed.Create(
            feedId,
            clock.UtcNow,
            userId,
            feedName,
            slug,
            FeedStatus.Active);

        user.AddFeed(feed);

        userCommandRepository.Update(user);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return slug;
    }
}