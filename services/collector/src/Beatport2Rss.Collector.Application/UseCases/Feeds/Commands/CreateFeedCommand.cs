using Beatport2Rss.Collector.Application.Interfaces.Persistence.Repositories;
using Beatport2Rss.Collector.Domain.Feeds;
using Beatport2Rss.Common.Domain.Feeds;
using Beatport2Rss.Common.Domain.Users;
using Beatport2Rss.Common.EntityFrameworkCore.Persistence.Interfaces;
using Beatport2Rss.Common.Miscellaneous.Exceptions;

namespace Beatport2Rss.Collector.Application.UseCases.Feeds.Commands;

public sealed record CreateFeedCommand(Guid FeedId, DateTimeOffset CreatedAt, Guid UserId);

public sealed class CreateFeedCommandHandler(
    IFeedCommandRepository feedCommandRepository,
    IUserCommandRepository userCommandRepository,
    IUnitOfWork unitOfWork)
{
    public async Task HandleAsync(CreateFeedCommand command, CancellationToken cancellationToken)
    {
        var feedId = FeedId.Create(command.FeedId);
        if (await feedCommandRepository.ExistsAsync(feedId, cancellationToken))
        {
            return;
        }

        var userId = UserId.Create(command.UserId);
        if (!await userCommandRepository.ExistsAsync(userId, cancellationToken))
        {
            throw new RetryableException($"User {command.UserId} is not available yet.");
        }

        var feed = Feed.Create(
            feedId,
            command.CreatedAt,
            userId);
        await feedCommandRepository.AddAsync(feed, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}