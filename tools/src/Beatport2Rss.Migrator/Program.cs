#pragma warning disable CA1303 // Do not pass literals as localized parameters

using System.Reflection;

using Beatport2Rss.Migrator;
using Beatport2Rss.Migrator.Enums;

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

if (!Enum.TryParse<ServiceName>(args.FirstOrDefault(), ignoreCase: true, out var serviceName))
{
    throw new ArgumentException("Service name cannot be inferred from arguments");
}

Func<IServiceCollection, IConfigurationRoot, IServiceCollection> addMigrator = serviceName switch
{
    ServiceName.Api => Beatport2Rss.Api.Infrastructure.ServiceCollectionExtensions.AddMigrator,
    ServiceName.Builder => Beatport2Rss.Builder.Infrastructure.ServiceCollectionExtensions.AddMigrator,
    ServiceName.Collector => Beatport2Rss.Collector.Infrastructure.ServiceCollectionExtensions.AddMigrator,
    _ => throw new NotSupportedException($"Service {serviceName} is not supported")
};

var serviceProvider = addMigrator(serviceCollection, configurationRoot)
    .AddLogging(builder => builder.AddConsole())
    .AddSingleton<IApplication, Application>()
    .BuildServiceProvider();

await serviceProvider.GetRequiredService<IApplication>().RunAsync();