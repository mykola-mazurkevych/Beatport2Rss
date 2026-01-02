using Beatport2Rss.Application.Interfaces.Persistence;
using Beatport2Rss.Application.Interfaces.Persistence.Repositories;
using Beatport2Rss.Application.Interfaces.Services;
using Beatport2Rss.Domain.Feeds;
using Beatport2Rss.Domain.Users;

using FluentValidation;

namespace Beatport2Rss.Application.UseCases.Feeds.Commands;

public readonly record struct CreateFeedCommand(
    Guid UserId,
    string? Name);

public sealed class CreateFeedCommandValidator :
    AbstractValidator<CreateFeedCommand>
{
    public CreateFeedCommandValidator(
        IFeedNameAvailabilityChecker feedNameAvailabilityChecker,
        IUserExistenceChecker userExistenceChecker)
    {
        RuleFor(c => c.UserId)
            .NotEmpty().WithMessage("User ID is required.")
            .MustAsync(userExistenceChecker.ExistsAsync).WithMessage("User does not exist.");

        RuleFor(c => c.Name)
            .NotEmpty().WithMessage("Feed name is required.")
            .MaximumLength(FeedName.MaxLength).WithMessage($"Feed name must be at most {FeedName.MaxLength} characters.")
            .MustAsync((c, name, cancellationToken) => feedNameAvailabilityChecker.IsAvailableAsync(c.UserId, name, cancellationToken)).WithMessage("Feed name already taken.");
    }
}

public sealed class CreateFeedCommandHandler(
    IClock clock,
    ISlugGenerator slugGenerator,
    IUserCommandRepository userCommandRepository,
    IUserQueryRepository userQueryRepository,
    IUnitOfWork unitOfWork)
{
    public async Task HandleAsync(CreateFeedCommand command, CancellationToken cancellationToken = default)
    {
        var userId = UserId.Create(command.UserId);
        var user = await userQueryRepository.GetAsync(userId, cancellationToken);

        var feedName = FeedName.Create(command.Name);

        var feed = Feed.Create(
            FeedId.Create(Guid.CreateVersion7()),
            feedName,
            slugGenerator.Generate(feedName),
            FeedStatus.Active,
            clock.UtcNow,
            userId);

        user!.AddFeed(feed);
        userCommandRepository.Update(user);

        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}