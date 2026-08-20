using Beatport2Rss.Api.Application.Extensions;
using Beatport2Rss.Api.Application.Interfaces.Messages;
using Beatport2Rss.Api.Application.Interfaces.Persistence.Repositories;
using Beatport2Rss.Api.Application.Interfaces.Services.Misc;
using Beatport2Rss.Api.Application.Interfaces.Services.Messaging;
using Beatport2Rss.Api.Domain.Feeds;
using Beatport2Rss.Api.Domain.Users;
using Beatport2Rss.Common.EntityFrameworkCore.Interfaces;
using Beatport2Rss.Common.IntegrationEvents.V1;
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
    string? AuthorName,
    bool UpdateSlug,
    bool IsActive) :
    ICommand<Result<Slug>>, IRequireUser, IRequireFeed;

internal sealed class UpdateFeedCommandValidator :
    AbstractValidator<UpdateFeedCommand>
{
    public UpdateFeedCommandValidator()
    {
        RuleFor(c => c.Name).IsFeedName();
        RuleFor(c => c.AuthorName).NotEmpty().MaximumLength(AuthorName.MaxLength).When(c => c.AuthorName is not null);
    }
}

internal sealed class UpdateFeedCommandHandler(
    IClock clock,
    ISlugGenerator slugGenerator,
    IFeedCommandRepository feedCommandRepository,
    IIntegrationEventOutbox integrationEventOutbox,
    IUnitOfWork unitOfWork) :
    ICommandHandler<UpdateFeedCommand, Result<Slug>>
{
    public async ValueTask<Result<Slug>> Handle(
        UpdateFeedCommand command,
        CancellationToken cancellationToken)
    {
        var feed = await feedCommandRepository.LoadAsync(command.UserId, command.FeedSlug, cancellationToken);

        var feedName = FeedName.Create(command.Name);
        var authorName = command.AuthorName is null
            ? (AuthorName?)null
            : AuthorName.Create(command.AuthorName);
        var slug = command.UpdateSlug
            ? slugGenerator.Generate(feedName.Value)
            : feed.Slug;

        if (await feedCommandRepository.ExistsExceptAsync(command.UserId, feedName, feed.Id, cancellationToken))
        {
            return Result.Conflict($"Feed name '{feedName}' is already taken.");
        }

        feed.UpdateName(feedName);
        feed.UpdateSlug(slug);
        feed.UpdateAuthorName(authorName);
        feed.UpdateStatus(command.IsActive);
        feedCommandRepository.Update(feed);

        var feedUpdated = new FeedUpdatedV1(
            EventId: Guid.CreateVersion7(),
            OccurredAt: clock.UtcNow,
            feed.Id.Value,
            feed.Name.Value,
            feed.Slug.Value,
            feed.AuthorName?.Value,
            feed.Status == FeedStatus.Active);
        integrationEventOutbox.Enqueue(feedUpdated);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return feed.Slug;
    }
}