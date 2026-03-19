#pragma warning disable CA1034 // Nested types should not be visible

using Beatport2Rss.Jobs.Jobs;

using Microsoft.Extensions.DependencyInjection;

using Quartz;

namespace Beatport2Rss.Jobs;

public static class ServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddJobs() =>
            services
                .AddQuartz(configurator =>
                {
                    var jobKey = new JobKey(nameof(DeleteExpiredSessionsJob));

                    configurator
                        .AddJob<DeleteExpiredSessionsJob>(jobKey, _ => { })
                        .AddTrigger(trigger => trigger.ForJob(jobKey).StartNow().WithSimpleSchedule(builder => builder.WithIntervalInHours(24).RepeatForever()));
                })
                .AddQuartzHostedService(options => options.WaitForJobsToComplete = true);
    }
}