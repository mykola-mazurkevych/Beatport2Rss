#pragma warning disable CA1303 // Do not pass literals as localized parameters

using System.Reflection;

using Beatport2Rss.Migrator;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

Console.WriteLine("#################################################");
Console.WriteLine("#    Beatport2Rss: Database Migration Runner    #");
Console.WriteLine("#################################################");

Console.WriteLine();

var configurationRoot = new ConfigurationBuilder()
    .AddUserSecrets(Assembly.GetEntryAssembly()!, optional: false)
    .Build();

var serviceCollection = new ServiceCollection();

Beatport2Rss.Infrastructure.ServiceCollectionExtensions.AddMigrator(serviceCollection, configurationRoot);
Beatport2Rss.ReleaseCollector.Infrastructure.ServiceCollectionExtensions.AddMigrator(serviceCollection, configurationRoot);

var serviceProvider =serviceCollection 
    .AddLogging(builder => builder.AddConsole())
    .AddSingleton<IApplication, Application>()
    .BuildServiceProvider();

await serviceProvider.GetRequiredService<IApplication>().RunAsync();