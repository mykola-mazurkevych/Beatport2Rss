using Beatport2Rss.Builder.Application.Interfaces.Persistence.Repositories;
using Beatport2Rss.Builder.Domain.Feeds;
using Beatport2Rss.Common.Domain.Common;
using Beatport2Rss.Common.Domain.Feeds;
using Beatport2Rss.Common.EntityFrameworkCore.Persistence.Interfaces;

using Microsoft.Extensions.Logging;

namespace Beatport2Rss.Builder.Application.UseCases.Feeds.Commands;

public sealed record CreateFeedCommand(
    Guid FeedId,
    DateTimeOffset CreatedAt,
    string Name,
    string Slug,
    string? AuthorName,
    FeedStatus Status);

public sealed class CreateFeedCommandHandler(
    IFeedCommandRepository feedCommandRepository,
    IUnitOfWork unitOfWork,
    ILogger<CreateFeedCommand> logger)
{
    public async Task Handle(
        CreateFeedCommand command,
        CancellationToken cancellationToken)
    {
        LoggerMessages.Creating(logger, command.FeedId, command.Name);

        var feedId = FeedId.Create(command.FeedId);
        if (await feedCommandRepository.ExistsAsync(feedId, cancellationToken))
        {
            LoggerMessages.AlreadyExists(logger, command.FeedId);
            return;
        }

        var feed = Feed.Create(
            feedId,
            command.CreatedAt,
            FeedName.Create(command.Name),
            Slug.Create(command.Slug),
            command.AuthorName is null ? null : AuthorName.Create(command.AuthorName),
            command.Status);

        await feedCommandRepository.AddAsync(feed, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        LoggerMessages.Created(logger, command.FeedId);
    }
}

internal static partial class LoggerMessages
{
    [LoggerMessage(Level = LogLevel.Information, Message = "Creating feed {FeedId} (Name: {FeedName})")]
    public static partial void Creating(ILogger logger, Guid feedId, string feedName);

    [LoggerMessage(Level = LogLevel.Warning, Message = "Feed {FeedId} already exists")]
    public static partial void AlreadyExists(ILogger logger, Guid feedId);

    [LoggerMessage(Level = LogLevel.Information, Message = "Feed {FeedId} created")]
    public static partial void Created(ILogger logger, Guid feedId);
}