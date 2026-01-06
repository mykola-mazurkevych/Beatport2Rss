using Beatport2Rss.Application.Extensions;
using Beatport2Rss.Application.Interfaces.Persistence;
using Beatport2Rss.Application.Interfaces.Persistence.Repositories;
using Beatport2Rss.Application.Interfaces.Services.Misc;
using Beatport2Rss.Application.Results;
using Beatport2Rss.Domain.Feeds;
using Beatport2Rss.Domain.Users;

using FluentValidation;

using OneOf;

namespace Beatport2Rss.Application.UseCases.Feeds.Commands;

public readonly record struct CreateFeedCommand(
    Guid UserId,
    string? Name);

public sealed class CreateFeedCommandValidator :
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

public sealed class CreateFeedCommandHandler(
    IValidator<CreateFeedCommand> validator,
    IClock clock,
    ISlugGenerator slugGenerator,
    IUserCommandRepository userRepository,
    IUnitOfWork unitOfWork)
{
    public async Task<OneOf<Success<Guid>, ValidationFailed, InactiveUser, Conflict>> HandleAsync(CreateFeedCommand command, CancellationToken cancellationToken = default)
    {
        var validationResult = await validator.ValidateAsync(command, cancellationToken);
        if (!validationResult.IsValid)
        {
            return new ValidationFailed(validationResult.GetErrors());
        }

        var userId = UserId.Create(command.UserId);
        var user = await userRepository.LoadWithFeedsAsync(userId, cancellationToken);

        if (!user.IsActive)
        {
            return new InactiveUser();
        }

        var feedId = FeedId.Create(Guid.CreateVersion7());
        var feedName = FeedName.Create(command.Name);
        var slug = slugGenerator.Generate(feedName);

        if (user.Feeds.Any(f => f.Name == feedName))
        {
            return new Conflict("Feed name is already taken.");
        }

        var feed = Feed.Create(
            feedId,
            feedName,
            slug,
            FeedStatus.Active,
            clock.UtcNow);

        user.AddFeed(feed);
        userRepository.Update(user);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new Success<Guid>(feedId);
    }
}