using Beatport2Rss.Application.Extensions;
using Beatport2Rss.Application.Interfaces.Messages;
using Beatport2Rss.Application.Interfaces.Persistence;
using Beatport2Rss.Application.Interfaces.Persistence.Repositories;
using Beatport2Rss.Domain.Common.ValueObjects;
using Beatport2Rss.Domain.Users;

using FluentResults;

using FluentValidation;

using Mediator;

namespace Beatport2Rss.Application.UseCases.Feeds.Commands;

public readonly record struct DeleteFeedCommand(
    Guid UserId,
    string Slug) :
    ICommand<Result>, IRequireActiveUser;

internal sealed class DeleteFeedCommandValidator :
    AbstractValidator<DeleteFeedCommand>
{
    public DeleteFeedCommandValidator()
    {
        RuleFor(c => c.UserId)
            .NotEmpty().WithMessage("User ID is required.");

        RuleFor(q => q.Slug)
            .NotEmpty().WithMessage("Slug is required.")
            .MaximumLength(Slug.MaxLength).WithMessage($"Slug must be at most {Slug.MaxLength} characters.");
    }
}

internal sealed class DeleteFeedCommandHandler(
    IUserCommandRepository userRepository,
    IUnitOfWork unitOfWork) :
    ICommandHandler<DeleteFeedCommand, Result>
{
    public async ValueTask<Result> Handle(
        DeleteFeedCommand command,
        CancellationToken cancellationToken)
    {
        var userId = UserId.Create(command.UserId);
        var user = await userRepository.LoadWithFeedsAsync(userId, cancellationToken);

        var feed = user.Feeds.SingleOrDefault(f => f.Slug == command.Slug);

        if (feed is null)
        {
            return Result.NotFound($"Feed with slug '{command.Slug}' was not found.");
        }

        user.RemoveFeed(feed);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok();
    }
}