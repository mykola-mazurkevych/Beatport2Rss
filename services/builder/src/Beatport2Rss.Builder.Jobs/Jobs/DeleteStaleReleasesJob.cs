using Beatport2Rss.Builder.Application.UseCases.Releases.Commands;
using Beatport2Rss.Builder.Jobs.Options;

using Microsoft.Extensions.Options;

using Quartz;

using Wolverine;

namespace Beatport2Rss.Builder.Jobs.Jobs;

internal sealed class DeleteStaleReleasesJob(
    IMessageBus messageBus,
    IOptions<BuilderJobOptions> options) :
    IJob
{
    private readonly BuilderJobOptions _options = options.Value;

    public async Task Execute(IJobExecutionContext context)
    {
        var command = new DeleteStaleReleasesCommand(_options.ReleasesRetentionInDays);
        await messageBus.InvokeAsync(command, context.CancellationToken);
    }
}