using Beatport2Rss.Builder.Application.Interfaces.Persistence.Repositories;
using Beatport2Rss.Common.Miscellaneous.Interfaces;
using Beatport2Rss.Common.Validation.Extensions;

using FluentValidation;

namespace Beatport2Rss.Builder.Application.UseCases.Releases.Commands;

public sealed record DeleteStaleReleasesCommand(
    int RetentionInDays);

public sealed class DeleteStaleReleasesCommandValidator :
    AbstractValidator<DeleteStaleReleasesCommand>
{
    public DeleteStaleReleasesCommandValidator()
    {
        RuleFor(c => c.RetentionInDays).IsPositive();
    }
}

public static class DeleteStaleReleasesCommandHandler
{
    public static async Task Handle(
        DeleteStaleReleasesCommand command,
        IClock clock,
        IReleaseCommandRepository releaseCommandRepository,
        CancellationToken cancellationToken)
    {
        var minCreatedAt = clock.UtcNow.AddDays(-command.RetentionInDays);
        await releaseCommandRepository.DeleteAsync(release => release.CreatedAt < minCreatedAt, cancellationToken);
    }
}