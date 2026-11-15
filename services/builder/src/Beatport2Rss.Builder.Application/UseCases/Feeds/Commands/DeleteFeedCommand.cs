using Beatport2Rss.Builder.Application.Interfaces.Persistence.Repositories;
using Beatport2Rss.Common.Domain.Feeds;
using Beatport2Rss.Common.EntityFrameworkCore.Persistence.Interfaces;

namespace Beatport2Rss.Builder.Application.UseCases.Feeds.Commands;

public sealed record DeleteFeedCommand(Guid FeedId);

public sealed class DeleteFeedCommandHandler(
    IFeedCommandRepository feedCommandRepository,
    IUnitOfWork unitOfWork)
{
    public async Task Handle(
        DeleteFeedCommand command,
        CancellationToken cancellationToken)
    {
        var feed = await feedCommandRepository.FindAsync(FeedId.Create(command.FeedId), cancellationToken);
        if (feed is null)
        {
            return;
        }

        feedCommandRepository.Delete(feed);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}