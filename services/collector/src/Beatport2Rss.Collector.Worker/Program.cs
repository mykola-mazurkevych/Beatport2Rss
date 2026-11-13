using Beatport2Rss.Collector.Application;
using Beatport2Rss.Collector.Infrastructure;
using Beatport2Rss.Collector.Jobs;

await Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) => services
        .AddApplication()
        .AddInfrastructure(context.Configuration)
        .AddCollectorJobs(context.Configuration))
    .RunConsoleAsync();