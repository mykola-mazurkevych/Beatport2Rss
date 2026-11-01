#pragma warning disable CA1034 // Nested types should not be visible

using Beatport2Rss.Api.Jobs.Jobs;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Quartz;

namespace Beatport2Rss.Api.Jobs;

public static class ServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddJobs(IConfiguration configuration) =>
            services
                .AddQuartz(configurator =>
                {
                    var deleteExpiredSessionsJobKey = new JobKey(nameof(DeleteExpiredSessionsJob));
                    configurator
                        .AddJob<DeleteExpiredSessionsJob>(job => job.WithIdentity(deleteExpiredSessionsJobKey))
                        .AddTrigger(trigger => trigger.ForJob(deleteExpiredSessionsJobKey).StartNow().WithSimpleSchedule(builder => builder.WithInterval(TimeSpan.FromHours(24)).RepeatForever()));
                })
                .AddQuartzHostedService(options => options.WaitForJobsToComplete = true);
    }
}