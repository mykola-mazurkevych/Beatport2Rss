#pragma warning disable CA1303 // Do not pass literals as localized parameters

using System.Reflection;

using Beatport2Rss.Application;
using Beatport2Rss.Infrastructure;
using Beatport2Rss.TokenProvider;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

Console.WriteLine("######################################################");
Console.WriteLine("#    Beatport2Rss: Beatport Access Token Provider    #");
Console.WriteLine("######################################################");

Console.WriteLine();

var configurationRoot = new ConfigurationBuilder()
    .AddUserSecrets(Assembly.GetEntryAssembly()!, optional: false)
    .Build();

var serviceProvider = new ServiceCollection()
    .AddLogging(builder => builder.AddConsole())
    .AddApplication()
    .AddInfrastructure(configurationRoot)
    .AddSingleton<IApplication, Application>()
    .BuildServiceProvider();

await serviceProvider.GetRequiredService<IApplication>().RunAsync();