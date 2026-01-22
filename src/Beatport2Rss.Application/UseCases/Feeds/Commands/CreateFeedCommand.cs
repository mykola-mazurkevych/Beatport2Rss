using Beatport2Rss.Application.Extensions;
using Beatport2Rss.Application.Interfaces.Messages;
using Beatport2Rss.Application.Interfaces.Persistence;
using Beatport2Rss.Application.Interfaces.Persistence.Repositories;
using Beatport2Rss.Application.Interfaces.Services.Misc;
using Beatport2Rss.Domain.Common.ValueObjects;
using Beatport2Rss.Domain.Feeds;
using Beatport2Rss.Domain.Users;

using FluentResults;

using FluentValidation;

using Mediator;

namespace Beatport2Rss.Application.UseCases.Feeds.Commands;

public readonly record struct CreateFeedCommand(
    Guid UserId,
    string? Name) :
    ICommand<Result<Slug>>, IRequireActiveUser;

internal sealed class CreateFeedCommandValidator :
    AbstractValidator<CreateFeedCommand>
{
    public CreateFeedCommandValidator()
    {
        RuleFor(c => c.UserId)
            .NotEmpty().WithMessage("User ID is required.");

        RuleFor(c => c.Name)
            .NotEmpty().WithMessage("Feed name is required.")
            .MaximumLength(FeedName.MaxLength).WithMessage($"Feed name must be at most {FeedName.MaxLength} characters.");
    }
}

internal sealed class CreateFeedCommandHandler(
    IClock clock,
    ISlugGenerator slugGenerator,
    IUserCommandRepository userRepository,
    IUnitOfWork unitOfWork) :
    ICommandHandler<CreateFeedCommand, Result<Slug>>
{
    public async ValueTask<Result<Slug>> Handle(
        CreateFeedCommand command,
        CancellationToken cancellationToken = default)
    {
        var userId = UserId.Create(command.UserId);
        var user = await userRepository.LoadWithFeedsAsync(userId, cancellationToken);

        var feedId = FeedId.Create(Guid.CreateVersion7());
        var feedName = FeedName.Create(command.Name);
        var slug = slugGenerator.Generate(feedName);

        if (user.Feeds.Any(f => f.Name == feedName))
        {
            return Result.Conflict($"Feed name '{feedName}' is already taken.");
        }

        var feed = Feed.Create(
            feedId,
            clock.UtcNow,
            feedName,
            slug,
            FeedStatus.Active);

        user.AddFeed(feed);

        userRepository.Update(user);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return slug;
    }
}