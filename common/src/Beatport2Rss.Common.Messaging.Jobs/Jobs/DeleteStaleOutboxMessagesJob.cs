using Beatport2Rss.Common.Messaging.Jobs.Options;
using Beatport2Rss.Common.Messaging.Persistence.Interfaces.Repositories;
using Beatport2Rss.Common.Miscellaneous.Interfaces;

using Microsoft.Extensions.Options;

using Quartz;

namespace Beatport2Rss.Common.Messaging.Jobs.Jobs;

internal sealed class DeleteStaleOutboxMessagesJob(
    IClock clock,
    IOutboxMessageRepository outboxMessageRepository,
    IOptions<OutboxJobOptions> outboxJobOptions) :
    IJob
{
    private readonly OutboxJobOptions _outboxJobOptions = outboxJobOptions.Value;

    public Task Execute(IJobExecutionContext context) =>
        outboxMessageRepository.DeleteAsync(outboxMessage =>
                outboxMessage.PublishedAt.HasValue &&
                outboxMessage.PublishedAt < clock.UtcNow.AddDays(-_outboxJobOptions.OutboxMessagesRetentionInDays),
            context.CancellationToken);
}