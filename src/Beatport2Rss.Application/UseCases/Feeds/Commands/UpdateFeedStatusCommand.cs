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

public readonly record struct UpdateFeedStatusCommand(
    Guid UserId,
    string? Slug,
    bool IsActive) :
    ICommand<Result>, IRequireActiveUser;

internal sealed class UpdateFeedStatusCommandValidator :
    AbstractValidator<UpdateFeedStatusCommand>
{
    public UpdateFeedStatusCommandValidator()
    {
        RuleFor(c => c.UserId)
            .NotEmpty().WithMessage("User ID is required.");

        RuleFor(c => c.Slug)
            .NotEmpty().WithMessage("Slug is required.")
            .MaximumLength(Slug.MaxLength).WithMessage($"Slug must be at most {Slug.MaxLength} characters.");
    }
}

internal sealed class UpdateFeedStatusCommandHandler(
    IUserCommandRepository userRepository,
    IUnitOfWork unitOfWork) :
    ICommandHandler<UpdateFeedStatusCommand, Result>
{
    public async ValueTask<Result> Handle(
        UpdateFeedStatusCommand command,
        CancellationToken cancellationToken)
    {
        var userId = UserId.Create(command.UserId);
        var user = await userRepository.LoadWithFeedsAsync(userId, cancellationToken);

        var slug = Slug.Create(command.Slug);

        if (!user.HasFeed(slug))
        {
            return Result.NotFound($"Feed with slug '{slug}' was not found.");
        }

        user.UpdateFeedStatus(slug, command.IsActive);

        userRepository.Update(user);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok();
    }
}