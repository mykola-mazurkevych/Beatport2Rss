using Microsoft.EntityFrameworkCore.Migrations;

namespace Beatport2Rss.Common.EntityFrameworkCore.Extensions;

public enum ViewOperation
{
    Up,
    Down,
}

public static class MigrationBuilderExtensions
{
    extension(MigrationBuilder migrationBuilder)
    {
        public void UpdateView(Type resourceMarker, string viewName, ViewOperation operation)
        {
            var resourceNamespace =
                resourceMarker.Namespace ??
                throw new ArgumentException("Resource marker must have a namespace.", nameof(resourceMarker));
            var resourceName = $"{resourceNamespace}.Views.{viewName}.{operation}.sql";

            using var stream =
                resourceMarker.Assembly.GetManifestResourceStream(resourceName) ??
                throw new InvalidOperationException($"Cannot find embedded SQL script: {resourceName}");

            using var streamReader = new StreamReader(stream);
            migrationBuilder.Sql(streamReader.ReadToEnd());
        }
    }
}