using Beatport2Rss.Common.EntityFrameworkCore.Persistence.Interfaces;

using Microsoft.EntityFrameworkCore.Migrations;

namespace Beatport2Rss.Common.EntityFrameworkCore.Extensions;

public enum MigrationDirection
{
    Up,
    Down,
}

public static class MigrationBuilderExtensions
{
    extension(MigrationBuilder migrationBuilder)
    {
        public void UpdateView<TDbContext>(string viewName, MigrationDirection direction)
            where TDbContext : IBeatport2RssDbContext
        {
            var dbContextType = typeof(TDbContext);
            var resourceNamespace =
                dbContextType.Namespace ??
                throw new InvalidOperationException("Db Context does not have a namespace");
            var resourceName = $"{resourceNamespace}.Views.{viewName}.{direction}.sql";

            using var stream =
                dbContextType.Assembly.GetManifestResourceStream(resourceName) ??
                throw new InvalidOperationException($"Cannot find embedded SQL script: {resourceName}");

            using var streamReader = new StreamReader(stream);
            migrationBuilder.Sql(streamReader.ReadToEnd());
        }
    }
}