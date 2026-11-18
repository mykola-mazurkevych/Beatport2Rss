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
    .AddUserSecrets(Assembly.GetEntryAssembly()!, optional: true)
    .AddEnvironmentVariables()
    .Build();

var serviceCollection = new ServiceCollection();

if (args.Length != 1)
{
    throw new ArgumentException("Expected one service argument: api, builder, or collector.", nameof(args));
}

switch (args[0])
{
    case "api":
        Beatport2Rss.Api.Infrastructure.ServiceCollectionExtensions.AddMigrator(serviceCollection, configurationRoot);
        break;
    case "builder":
        Beatport2Rss.Builder.Infrastructure.ServiceCollectionExtensions.AddMigrator(serviceCollection, configurationRoot);
        break;
    case "collector":
        Beatport2Rss.Collector.Infrastructure.ServiceCollectionExtensions.AddMigrator(serviceCollection, configurationRoot);
        break;
    default:
        throw new ArgumentException($"Unsupported service '{args[0]}'. Expected api, builder, or collector.", nameof(args));
}

var serviceProvider = serviceCollection
    .AddLogging(builder => builder.AddConsole())
    .AddSingleton<IApplication, Application>()
    .BuildServiceProvider();

await serviceProvider.GetRequiredService<IApplication>().RunAsync();