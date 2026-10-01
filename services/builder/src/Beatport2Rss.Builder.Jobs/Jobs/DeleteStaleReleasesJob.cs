using Beatport2Rss.Builder.Application.UseCases.Releases.Commands;
using Beatport2Rss.Builder.Jobs.Options;

using Mediator;

using Microsoft.Extensions.Options;

using Quartz;

namespace Beatport2Rss.Builder.Jobs.Jobs;

internal sealed class DeleteStaleReleasesJob(
    IMediator mediator,
    IOptions<BuilderJobOptions> options) :
    IJob
{
    private readonly BuilderJobOptions _options = options.Value;

    public async Task Execute(IJobExecutionContext context)
    {
        var command = new DeleteStaleReleasesCommand(_options.ReleasesRetentionInDays);
        await mediator.Send(command, context.CancellationToken);
    }
}