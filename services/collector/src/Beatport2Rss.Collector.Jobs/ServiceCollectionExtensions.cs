#pragma warning disable CA1034 // Nested types should not be visible

using Beatport2Rss.Collector.Jobs.Jobs;
using Beatport2Rss.Collector.Jobs.Options;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using Quartz;

namespace Beatport2Rss.Collector.Jobs;

public static class ServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddCollectorJobs(IConfiguration configuration) =>
            services
                .ConfigureOptions(configuration)
                .AddQuartz(configurator =>
                {
                    var collectReleasesJobKey = new JobKey(nameof(CollectReleasesJob));
                    configurator
                        .AddJob<CollectReleasesJob>(job => job.WithIdentity(collectReleasesJobKey))
                        .AddTrigger((provider, trigger) =>
                        {
                            var options = provider.GetRequiredService<IOptions<CollectorJobOptions>>().Value;

                            trigger
                                .ForJob(collectReleasesJobKey)
                                .WithCronSchedule(options.CollectReleasesCronSchedule);
                        });
                })
                .AddQuartzHostedService(options => options.WaitForJobsToComplete = true);

        private IServiceCollection ConfigureOptions(IConfiguration configuration) =>
            services.Configure<CollectorJobOptions>(configuration.GetSection(nameof(CollectorJobOptions)));
    }
}