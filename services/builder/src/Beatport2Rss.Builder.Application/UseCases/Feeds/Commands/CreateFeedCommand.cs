using Beatport2Rss.Builder.Application.Interfaces.Persistence.Repositories;
using Beatport2Rss.Builder.Domain.Feeds;
using Beatport2Rss.Common.EntityFrameworkCore.Persistence.Interfaces;
using Beatport2Rss.Common.Domain.ValueObjects;
using Beatport2Rss.Common.Validation.Extensions;

using FluentValidation;

namespace Beatport2Rss.Builder.Application.UseCases.Feeds.Commands;

public sealed record CreateFeedCommand(
    Guid FeedId,
    DateTimeOffset CreatedAt,
    string Name,
    string Slug,
    string? AuthorName,
    string Status);

public sealed class CreateFeedCommandValidator :
    AbstractValidator<CreateFeedCommand>
{
    public CreateFeedCommandValidator()
    {
        RuleFor(c => c.FeedId).NotEmpty();
        RuleFor(c => c.Name).IsFeedName();
        RuleFor(c => c.Slug).NotEmpty();
        RuleFor(c => c.AuthorName).NotEmpty().MaximumLength(AuthorName.MaxLength).When(c => c.AuthorName is not null);
        RuleFor(c => c.Status).Must(status => Enum.TryParse<FeedStatus>(status, out var parsed) && Enum.IsDefined(parsed));
    }
}

public static class CreateFeedCommandHandler
{
    public static async Task Handle(
        CreateFeedCommand command,
        IFeedCommandRepository feedCommandRepository,
        IUnitOfWork unitOfWork,
        CancellationToken cancellationToken)
    {
        var feedId = FeedId.Create(command.FeedId);
        if (await feedCommandRepository.ExistsAsync(feedId, cancellationToken))
        {
            return;
        }

        var feed = Feed.Create(
            feedId,
            command.CreatedAt,
            FeedName.Create(command.Name),
            Slug.Create(command.Slug),
            command.AuthorName is null ? null : AuthorName.Create(command.AuthorName),
            Enum.Parse<FeedStatus>(command.Status));

        await feedCommandRepository.AddAsync(feed, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}