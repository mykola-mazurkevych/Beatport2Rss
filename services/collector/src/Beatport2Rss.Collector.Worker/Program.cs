using Beatport2Rss.Collector.Application;
using Beatport2Rss.Collector.Infrastructure;

await Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) => services
        .AddApplication()
        .AddInfrastructure(context.Configuration))
    .RunConsoleAsync();