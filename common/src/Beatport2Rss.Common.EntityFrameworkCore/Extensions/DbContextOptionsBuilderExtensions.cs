using Microsoft.EntityFrameworkCore;

namespace Beatport2Rss.Common.EntityFrameworkCore.Extensions;

public static class DbContextOptionsBuilderExtensions
{
    private const string MigrationsHistoryTableName = "__EFMigrationsHistory";

    extension(DbContextOptionsBuilder builder)
    {
        public DbContextOptionsBuilder UseNpgsql(string? connectionString,
            string? schema) =>
            builder.UseNpgsql(
                connectionString,
                options => options.MigrationsHistoryTable(MigrationsHistoryTableName, schema));
    }
}