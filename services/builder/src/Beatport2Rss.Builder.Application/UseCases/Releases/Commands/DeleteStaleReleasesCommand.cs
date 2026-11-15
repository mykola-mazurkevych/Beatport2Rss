using Beatport2Rss.Builder.Application.Interfaces.Persistence.Repositories;
using Beatport2Rss.Common.Miscellaneous.Interfaces;

namespace Beatport2Rss.Builder.Application.UseCases.Releases.Commands;

public sealed record DeleteStaleReleasesCommand(
    int RetentionInDays);

public sealed class DeleteStaleReleasesCommandHandler(
    IClock clock,
    IReleaseCommandRepository releaseCommandRepository)
{
    public async Task Handle(
        DeleteStaleReleasesCommand command,
        CancellationToken cancellationToken)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(command.RetentionInDays);

        var minCreatedAt = clock.UtcNow.AddDays(-command.RetentionInDays);
        await releaseCommandRepository.DeleteAsync(release => release.CreatedAt < minCreatedAt, cancellationToken);
    }
}