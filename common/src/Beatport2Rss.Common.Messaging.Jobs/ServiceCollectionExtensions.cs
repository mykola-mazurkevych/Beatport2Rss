#pragma warning disable CA1034 // Nested types should not be visible

using Beatport2Rss.Common.Messaging.Jobs.Jobs;
using Beatport2Rss.Common.Messaging.Jobs.Options;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using Quartz;

namespace Beatport2Rss.Common.Messaging.Jobs;

public static class ServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddInboxJobs(IConfiguration configuration) =>
            services
                .Configure<InboxJobOptions>(options => configuration.GetSection(nameof(InboxJobOptions)).Bind(options))
                .AddQuartz((configurator, provider) =>
                {
                    var options = provider.GetRequiredService<IOptions<InboxJobOptions>>().Value;

                    var deleteStaleInboxMessagesJobKey = new JobKey(nameof(DeleteStaleInboxMessagesJob));
                    configurator
                        .AddJob<DeleteStaleInboxMessagesJob>(deleteStaleInboxMessagesJobKey)
                        .AddTrigger(trigger => trigger.ForJob(deleteStaleInboxMessagesJobKey).WithCronSchedule(options.DeleteStaleInboxMessagesCronSchedule));
                });

        public IServiceCollection AddOutboxJobs(IConfiguration configuration) =>
            services
                .Configure<OutboxJobOptions>(options => configuration.GetSection(nameof(OutboxJobOptions)).Bind(options))
                .AddQuartz((configurator, provider) =>
                {
                    var options = provider.GetRequiredService<IOptions<OutboxJobOptions>>().Value;

                    var deleteStaleOutboxMessagesJobKey = new JobKey(nameof(DeleteStaleOutboxMessagesJob));
                    configurator
                        .AddJob<DeleteStaleOutboxMessagesJob>(deleteStaleOutboxMessagesJobKey)
                        .AddTrigger(trigger => trigger.ForJob(deleteStaleOutboxMessagesJobKey).WithCronSchedule(options.DeleteStaleOutboxMessagesCronSchedule));
                });
    }
}