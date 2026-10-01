using Beatport2Rss.Builder.Application.Interfaces.Persistence.Repositories;
using Beatport2Rss.Common.Miscellaneous.Interfaces;
using Beatport2Rss.Common.Validation.Extensions;

using FluentResults;

using FluentValidation;

using Mediator;

namespace Beatport2Rss.Builder.Application.UseCases.Releases.Commands;

public sealed record DeleteStaleReleasesCommand(
    int RetentionInDays) :
    ICommand<Result>;

internal sealed class DeleteStaleReleasesCommandValidator :
    AbstractValidator<DeleteStaleReleasesCommand>
{
    public DeleteStaleReleasesCommandValidator()
    {
        RuleFor(c => c.RetentionInDays).IsPositive();
    }
}

internal sealed class DeleteStaleReleasesCommandHandler(
    IClock clock,
    IReleaseCommandRepository releaseCommandRepository) :
    ICommandHandler<DeleteStaleReleasesCommand, Result>
{
    public async ValueTask<Result> Handle(
        DeleteStaleReleasesCommand command,
        CancellationToken cancellationToken)
    {
        var minCreatedAt = clock.UtcNow.AddDays(-command.RetentionInDays);
        await releaseCommandRepository.DeleteAsync(release => release.CreatedAt < minCreatedAt, cancellationToken);

        return Result.Ok();
    }
}