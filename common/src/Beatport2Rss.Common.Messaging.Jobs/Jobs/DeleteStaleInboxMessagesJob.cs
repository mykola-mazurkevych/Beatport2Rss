using Beatport2Rss.Common.Messaging.Jobs.Options;
using Beatport2Rss.Common.Messaging.Persistence.Interfaces.Repositories;
using Beatport2Rss.Common.Miscellaneous.Interfaces;

using Microsoft.Extensions.Options;

using Quartz;

namespace Beatport2Rss.Common.Messaging.Jobs.Jobs;

internal sealed class DeleteStaleInboxMessagesJob(
    IClock clock,
    IInboxMessageRepository inboxMessageRepository,
    IOptions<InboxJobOptions> inboxJobOptions) :
    IJob
{
    private readonly InboxJobOptions _inboxJobOptions = inboxJobOptions.Value;

    public Task Execute(IJobExecutionContext context) =>
        inboxMessageRepository.DeleteAsync(inboxMessage =>
                inboxMessage.ProcessedAt.HasValue &&
                inboxMessage.ProcessedAt < clock.UtcNow.AddDays(-_inboxJobOptions.InboxMessagesRetentionInDays),
            context.CancellationToken);
}