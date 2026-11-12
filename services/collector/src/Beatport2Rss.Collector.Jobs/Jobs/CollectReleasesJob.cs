using Beatport2Rss.Collector.Application.UseCases.Releases.Commands;

using Quartz;

using Wolverine;

namespace Beatport2Rss.Collector.Jobs.Jobs;

[DisallowConcurrentExecution]
internal sealed class CollectReleasesJob(IMessageBus messageBus) :
    IJob
{
    public async ValueTask Execute(IJobExecutionContext context, CancellationToken cancellationToken) =>
        await messageBus.SendAsync(new QueueReleaseCollectionCommand());
}