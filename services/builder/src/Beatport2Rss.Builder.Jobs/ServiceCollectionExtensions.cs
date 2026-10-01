#pragma warning disable CA1034 // Nested types should not be visible

using Beatport2Rss.Builder.Jobs.Jobs;
using Beatport2Rss.Builder.Jobs.Options;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using Quartz;

namespace Beatport2Rss.Builder.Jobs;

public static class ServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddJobs(IConfiguration configuration) =>
            services
                .ConfigureOptions(configuration)
                .AddQuartz((configurator, provider) =>
                {
                    var options = provider.GetRequiredService<IOptions<BuilderJobOptions>>().Value;

                    var deleteStaleReleasesJobKey = new JobKey(nameof(DeleteStaleReleasesJob));
                    configurator
                        .AddJob<DeleteStaleReleasesJob>(deleteStaleReleasesJobKey)
                        .AddTrigger(trigger => trigger.ForJob(deleteStaleReleasesJobKey).WithCronSchedule(options.DeleteStaleReleasesCronSchedule));
                })
                .AddQuartzHostedService(options => options.WaitForJobsToComplete = true);

        private IServiceCollection ConfigureOptions(IConfiguration configuration) =>
            services
                .Configure<BuilderJobOptions>(options => configuration.GetSection(nameof(BuilderJobOptions)).Bind(options));
    }
}