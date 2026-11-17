using Beatport2Rss.Collector.Application.Interfaces.Persistence.Repositories;
using Beatport2Rss.Common.Domain.Feeds;
using Beatport2Rss.Common.EntityFrameworkCore.Persistence.Interfaces;

namespace Beatport2Rss.Collector.Application.UseCases.Feeds.Commands;

public sealed record DeleteFeedCommand(Guid FeedId);

public sealed class DeleteFeedCommandHandler(
    IFeedCommandRepository feedCommandRepository,
    IUnitOfWork unitOfWork)
{
    public async Task HandleAsync(DeleteFeedCommand command, CancellationToken cancellationToken)
    {
        var feedId = FeedId.Create(command.FeedId);
        var feed = await feedCommandRepository.FindAsync(feedId, cancellationToken);
        if (feed is null)
        {
            return;
        }

        feedCommandRepository.Delete(feed);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}