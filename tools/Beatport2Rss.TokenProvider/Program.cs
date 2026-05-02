#pragma warning disable CA1303 // Do not pass literals as localized parameters

// ReSharper disable AccessToDisposedClosure

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
    .AddLogging(builder => builder.AddSimpleConsole(options => options.TimestampFormat = Constants.DateTimeFormat))
    .AddApplication()
    .AddInfrastructure(configurationRoot)
    .AddSingleton<IApplication, Application>()
    .BuildServiceProvider();

var cancellationTokenSource = new CancellationTokenSource();

Console.CancelKeyPress += OnCancelKeyPress;

try
{
    await serviceProvider
        .GetRequiredService<IApplication>()
        .RunAsync(args, cancellationTokenSource.Token);
}
catch (OperationCanceledException)
    when (cancellationTokenSource.IsCancellationRequested)
{
}
finally
{
    Console.CancelKeyPress -= OnCancelKeyPress;
    cancellationTokenSource.Dispose();
}

return;

void OnCancelKeyPress(object? _, ConsoleCancelEventArgs e)
{
    e.Cancel = true;
    cancellationTokenSource.Cancel();
}