using Beatport2Rss.Builder.Application.Interfaces.Persistence.Repositories;
using Beatport2Rss.Common.Domain.Common;
using Beatport2Rss.Common.Domain.Feeds;
using Beatport2Rss.Common.EntityFrameworkCore.Persistence.Interfaces;

namespace Beatport2Rss.Builder.Application.UseCases.Feeds.Commands;

public sealed record UpdateFeedCommand(
    Guid FeedId,
    string Name,
    string Slug,
    string? AuthorName,
    FeedStatus Status);

public sealed class UpdateFeedCommandHandler(
    IFeedCommandRepository feedCommandRepository,
    IUnitOfWork unitOfWork)
{
    public async Task Handle(
        UpdateFeedCommand command,
        CancellationToken cancellationToken)
    {
        var feed = await feedCommandRepository.FindAsync(FeedId.Create(command.FeedId), cancellationToken);
        if (feed is null)
        {
            return;
        }

        feed.UpdateName(FeedName.Create(command.Name));
        feed.UpdateSlug(Slug.Create(command.Slug));
        feed.UpdateAuthorName(command.AuthorName is null ? null : AuthorName.Create(command.AuthorName));
        feed.UpdateStatus(command.Status);

        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}