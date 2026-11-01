using Beatport2Rss.Builder.Infrastructure;
using Beatport2Rss.Builder.Jobs;
using Beatport2Rss.Common.Miscellaneous;

await Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) => services
        .AddInfrastructure(context.Configuration)
        .AddMiscellaneous() // TODO: should be moved inside AddInfrastructure
        .AddJobs(context.Configuration))
    .RunConsoleAsync();
