using Beatport2Rss.Builder.Application.Interfaces.Persistence.Repositories;
using Beatport2Rss.Common.Domain.Feeds;
using Beatport2Rss.Common.EntityFrameworkCore.Persistence.Interfaces;

using FluentValidation;

namespace Beatport2Rss.Builder.Application.UseCases.Feeds.Commands;

public sealed record DeleteFeedCommand(Guid FeedId);

public sealed class DeleteFeedCommandValidator :
    AbstractValidator<DeleteFeedCommand>
{
    public DeleteFeedCommandValidator()
    {
        RuleFor(c => c.FeedId).NotEmpty();
    }
}

public static class DeleteFeedCommandHandler
{
    public static async Task Handle(
        DeleteFeedCommand command,
        IFeedCommandRepository feedCommandRepository,
        IUnitOfWork unitOfWork,
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