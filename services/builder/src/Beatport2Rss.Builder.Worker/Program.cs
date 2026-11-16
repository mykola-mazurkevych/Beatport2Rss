using Beatport2Rss.Builder.Infrastructure;
using Beatport2Rss.Builder.Jobs;

await Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) => services
        .AddInfrastructure(context.Configuration)
        .AddJobs(context.Configuration))
    .RunConsoleAsync();
