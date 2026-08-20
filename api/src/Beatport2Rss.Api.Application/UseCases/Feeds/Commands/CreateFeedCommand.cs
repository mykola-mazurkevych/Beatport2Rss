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

public sealed record CreateFeedCommand(
    UserId UserId,
    string? Name,
    string? AuthorName,
    bool IsActive) :
    ICommand<Result<Slug>>, IRequireValidation, IRequireActiveUser;

internal sealed class CreateFeedCommandValidator :
    AbstractValidator<CreateFeedCommand>
{
    public CreateFeedCommandValidator()
    {
        RuleFor(c => c.Name).IsFeedName();
        RuleFor(c => c.AuthorName).NotEmpty().MaximumLength(AuthorName.MaxLength).When(c => c.AuthorName is not null);
    }
}

internal sealed class CreateFeedCommandHandler(
    IClock clock,
    ISlugGenerator slugGenerator,
    IFeedCommandRepository feedCommandRepository,
    IIntegrationEventOutbox integrationEventOutbox,
    IUnitOfWork unitOfWork) :
    ICommandHandler<CreateFeedCommand, Result<Slug>>
{
    public async ValueTask<Result<Slug>> Handle(
        CreateFeedCommand command,
        CancellationToken cancellationToken = default)
    {
        var feedId = FeedId.Create(Guid.CreateVersion7());
        var feedName = FeedName.Create(command.Name);
        var authorName = command.AuthorName is null
            ? (AuthorName?)null
            : AuthorName.Create(command.AuthorName);
        var slug = slugGenerator.Generate(feedName.Value);

        if (await feedCommandRepository.ExistsAsync(command.UserId, feedName, cancellationToken))
        {
            return Result.Conflict($"Feed name '{feedName}' is already taken.");
        }

        var feed = Feed.Create(
            feedId,
            clock.UtcNow,
            command.UserId,
            feedName,
            slug,
            authorName,
            command.IsActive);
        await feedCommandRepository.AddAsync(feed, cancellationToken);

        var feedCreated = new FeedCreatedV1(
            EventId: Guid.CreateVersion7(),
            OccurredAt: clock.UtcNow,
            feed.Id.Value,
            feed.Name.Value,
            feed.Slug.Value,
            feed.AuthorName?.Value,
            feed.Status == FeedStatus.Active);
        integrationEventOutbox.Enqueue(feedCreated);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return slug;
    }
}