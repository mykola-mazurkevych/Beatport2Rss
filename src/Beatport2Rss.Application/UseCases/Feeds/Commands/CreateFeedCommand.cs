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

public sealed class CreateFeedCommandValidator : AbstractValidator<CreateFeedCommand>
{
    private readonly IQueryRepository<User, UserId> _userQueryRepository;

    public CreateFeedCommandValidator(IQueryRepository<User, UserId> userQueryRepository)
    {
        _userQueryRepository = userQueryRepository;

        RuleFor(c => c.UserId)
            .NotEmpty().WithMessage("User ID is required.")
            .MustAsync(UserExists).WithMessage("User does not exist.");

        RuleFor(c => c.Name)
            .NotEmpty().WithMessage("Feed name is required.")
            .MaximumLength(FeedName.MaxLength).WithMessage($"Feed name must be at most {FeedName.MaxLength} characters.")
            .Must(FeedNameNotTaken).WithMessage("Feed name already taken.");
    }

    private async Task<bool> UserExists(CreateFeedCommand command, Guid id, ValidationContext<CreateFeedCommand> validationContext, CancellationToken cancellationToken)
    {
        var userId = UserId.Create(id);
        var user = await _userQueryRepository.GetAsync(userId, cancellationToken);

        validationContext.RootContextData[nameof(User)] = user;

        return user is not null;
    }

    private bool FeedNameNotTaken(CreateFeedCommand command, string name, ValidationContext<CreateFeedCommand> validationContext)
    {
        var user = (User)validationContext.RootContextData[nameof(User)];
        var feedName = FeedName.Create(name);
        var feedExists = user.Feeds.Any(f => f.Name == feedName);

        return !feedExists;
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

        user!.AddFeed(feed);
        userCommandRepository.Update(user);

        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}