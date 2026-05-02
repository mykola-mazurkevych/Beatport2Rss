using System.Reflection;

using Microsoft.EntityFrameworkCore.Migrations;

namespace Beatport2Rss.Infrastructure.Persistence.Extensions;

internal static class MigrationBuilderExtensions
{
    private const string NamespacePrefix = "Beatport2Rss.Infrastructure.Persistence.Views";

    extension(MigrationBuilder migrationBuilder)
    {
        public void UpdateView(string viewName, Operation operation)
        {
            var resourceName = $"{NamespacePrefix}.{viewName}.{operation}.sql";

            using var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName) ??
                               throw new InvalidOperationException($"Cannot find embedded SQL script: {resourceName}");

            using var streamReader = new StreamReader(stream);
            migrationBuilder.Sql(streamReader.ReadToEnd());
        }
    }

    public enum Operation
    {
        Up,
        Down,
    }
}