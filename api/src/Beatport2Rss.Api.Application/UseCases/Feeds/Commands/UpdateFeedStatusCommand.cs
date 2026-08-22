using Beatport2Rss.Api.Application.Interfaces.Messages;
using Beatport2Rss.Api.Application.Interfaces.Persistence.Repositories;
using Beatport2Rss.Api.Application.Interfaces.Services.Messaging;
using Beatport2Rss.Api.Application.Interfaces.Services.Misc;
using Beatport2Rss.Api.Domain.Users;
using Beatport2Rss.Common.EntityFrameworkCore.Interfaces;
using Beatport2Rss.Common.IntegrationEvents.V1;
using Beatport2Rss.Common.SharedKernel.ValueObjects;

using FluentResults;

using Mediator;

namespace Beatport2Rss.Api.Application.UseCases.Feeds.Commands;

public sealed record UpdateFeedStatusCommand(
    UserId UserId,
    Slug FeedSlug,
    bool IsActive) :
    ICommand<Result>, IRequireActiveUser, IRequireFeed;

internal sealed class UpdateFeedStatusCommandHandler(
    IClock clock,
    IFeedCommandRepository feedCommandRepository,
    IIntegrationEventOutbox integrationEventOutbox,
    IUnitOfWork unitOfWork) :
    ICommandHandler<UpdateFeedStatusCommand, Result>
{
    public async ValueTask<Result> Handle(
        UpdateFeedStatusCommand command,
        CancellationToken cancellationToken)
    {
        var feed = await feedCommandRepository.LoadAsync(command.UserId, command.FeedSlug, cancellationToken);

        feed.UpdateStatus(command.IsActive);
        feedCommandRepository.Update(feed);

        var feedUpdated = new FeedUpdatedV1(
            EventId: Guid.CreateVersion7(),
            OccurredAt: clock.UtcNow,
            feed.Id.Value,
            feed.Name.Value,
            feed.Slug.Value,
            feed.AuthorName?.Value,
            feed.Status.ToString());
        integrationEventOutbox.Enqueue(feedUpdated);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok();
    }
}