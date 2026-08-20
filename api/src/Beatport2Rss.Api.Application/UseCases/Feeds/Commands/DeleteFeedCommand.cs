using Beatport2Rss.Api.Application.Interfaces.Messages;
using Beatport2Rss.Api.Application.Interfaces.Persistence.Repositories;
using Beatport2Rss.Api.Application.Interfaces.Services.Misc;
using Beatport2Rss.Api.Application.Interfaces.Services.Messaging;
using Beatport2Rss.Api.Domain.Users;
using Beatport2Rss.Common.EntityFrameworkCore.Interfaces;
using Beatport2Rss.Common.IntegrationEvents.V1;
using Beatport2Rss.Common.SharedKernel.ValueObjects;

using FluentResults;

using Mediator;

namespace Beatport2Rss.Api.Application.UseCases.Feeds.Commands;

public sealed record DeleteFeedCommand(
    UserId UserId,
    Slug FeedSlug) :
    ICommand<Result>, IRequireUser, IRequireFeed;

internal sealed class DeleteFeedCommandHandler(
    IClock clock,
    IFeedCommandRepository feedCommandRepository,
    IIntegrationEventOutbox integrationEventOutbox,
    IUnitOfWork unitOfWork) :
    ICommandHandler<DeleteFeedCommand, Result>
{
    public async ValueTask<Result> Handle(
        DeleteFeedCommand command,
        CancellationToken cancellationToken)
    {
        var feed = await feedCommandRepository.LoadAsync(command.UserId, command.FeedSlug, cancellationToken);
        feedCommandRepository.Delete(feed);

        var feedDeleted = new FeedDeletedV1(
            EventId: Guid.CreateVersion7(),
            OccurredAt: clock.UtcNow,
            feed.Id.Value);
        integrationEventOutbox.Enqueue(feedDeleted);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok();
    }
}