using Beatport2Rss.Builder.Application.Interfaces.Persistence.Repositories;
using Beatport2Rss.Common.Domain.Feeds;
using Beatport2Rss.Common.EntityFrameworkCore.Persistence.Interfaces;
using Beatport2Rss.Common.Domain.ValueObjects;
using Beatport2Rss.Common.Validation.Extensions;

using FluentValidation;

namespace Beatport2Rss.Builder.Application.UseCases.Feeds.Commands;

public sealed record UpdateFeedCommand(
    Guid FeedId,
    string Name,
    string Slug,
    string? AuthorName,
    string Status);

public sealed class UpdateFeedCommandValidator :
    AbstractValidator<UpdateFeedCommand>
{
    public UpdateFeedCommandValidator()
    {
        RuleFor(c => c.FeedId).NotEmpty();
        RuleFor(c => c.Name).IsFeedName();
        RuleFor(c => c.Slug).IsSlug();
        RuleFor(c => c.AuthorName).IsAuthorName();
        RuleFor(c => c.Status).IsEnum<UpdateFeedCommand, FeedStatus>();
    }
}

public static class UpdateFeedCommandHandler
{
    public static async Task Handle(
        UpdateFeedCommand command,
        IFeedCommandRepository feedCommandRepository,
        IUnitOfWork unitOfWork,
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
        feed.UpdateStatus(Enum.Parse<FeedStatus>(command.Status));

        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}