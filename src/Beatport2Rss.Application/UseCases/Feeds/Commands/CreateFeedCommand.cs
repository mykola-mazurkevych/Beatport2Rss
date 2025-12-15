using Beatport2Rss.Contracts.Interfaces;
using Beatport2Rss.Contracts.Persistence;
using Beatport2Rss.Contracts.Persistence.Repositories;
using Beatport2Rss.Domain.Feeds;
using Beatport2Rss.Domain.Users;

using FluentValidation;

namespace Beatport2Rss.Application.UseCases.Feeds.Commands;

public readonly record struct CreateFeedCommand(
    Guid UserId,
    string? Name);

public sealed class CreateFeedCommandValidator : AbstractValidator<CreateFeedCommand>
{
    public CreateFeedCommandValidator(IQueryRepository<User, UserId> userQueryRepository)
    {
        RuleFor(c => c.UserId)
            .NotEmpty().WithMessage("User ID is required.")
            .MustAsync(async (_, id, validationContext, cancellationToken) =>
            {
                var userId = UserId.Create(id);
                var userExists = await userQueryRepository.ExistsAsync(u => u.Id == userId, cancellationToken);
                if (userExists)
                {
                    validationContext.RootContextData["User"] = await userQueryRepository.GetAsync(userId, cancellationToken);
                }

                return userExists;
            })
            .WithMessage("User does not exist.");

        RuleFor(c => c.Name)
            .NotEmpty().WithMessage("Feed name is required.")
            .MaximumLength(FeedName.MaxLength).WithMessage($"Feed name must be at most {FeedName.MaxLength} characters.")
            .Must((_, name, validationContext) =>
            {
                var user = (User)validationContext.RootContextData["User"];
                var feedName = FeedName.Create(name);
                var feedExists = user.Feeds.Any(f => f.Name == feedName);

                return !feedExists;
            }).WithMessage("Feed name contains invalid characters.");
    }
}

public sealed class CreateFeedCommandHandler(
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
            DateTimeOffset.UtcNow,
            userId);

        user.AddFeed(feed);
        userCommandRepository.Update(user);

        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}