using Beatport2Rss.Collector.Infrastructure;

await Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) => services
        .AddInfrastructure(context.Configuration))
    .RunConsoleAsync();