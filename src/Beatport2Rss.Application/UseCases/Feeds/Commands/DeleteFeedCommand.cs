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
    string? Slug) :
    ICommand<Result>, IRequireActiveUser;

internal sealed class DeleteFeedCommandValidator :
    AbstractValidator<DeleteFeedCommand>
{
    public DeleteFeedCommandValidator()
    {
        RuleFor(c => c.UserId).IsUserId();
        RuleFor(c => c.Slug).IsSlug();
    }
}

internal sealed class DeleteFeedCommandHandler(
    IUserCommandRepository userCommandRepository,
    IUnitOfWork unitOfWork) :
    ICommandHandler<DeleteFeedCommand, Result>
{
    public async ValueTask<Result> Handle(
        DeleteFeedCommand command,
        CancellationToken cancellationToken)
    {
        var userId = UserId.Create(command.UserId);
        var user = await userCommandRepository.LoadWithFeedsAsync(userId, cancellationToken);

        var slug = Slug.Create(command.Slug);

        // TODO: Move to behavior
        if (!user.HasFeed(slug))
        {
            return Result.NotFound($"Feed with slug '{slug}' was not found.");
        }

        user.RemoveFeed(slug);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok();
    }
}