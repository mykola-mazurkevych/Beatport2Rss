using Microsoft.EntityFrameworkCore.Migrations;

namespace Beatport2Rss.Migrator;

internal interface IApplication
{
    Task RunAsync(CancellationToken cancellationToken = default);
}

internal sealed class Application(
    IMigrator migrator) :
    IApplication
{
    public Task RunAsync(CancellationToken cancellationToken = default) =>
        migrator.MigrateAsync(cancellationToken: cancellationToken);
}