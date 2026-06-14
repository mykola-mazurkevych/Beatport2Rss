using Microsoft.EntityFrameworkCore.Migrations;

namespace Beatport2Rss.Migrator;

internal interface IApplication
{
    Task RunAsync(CancellationToken cancellationToken = default);
}

internal sealed class Application(
    IEnumerable<IMigrator> migrators) :
    IApplication
{
    public async Task RunAsync(CancellationToken cancellationToken = default)
    {
        foreach (var migrator in migrators)
        {
            await migrator.MigrateAsync(cancellationToken: cancellationToken);
        }
    }
}