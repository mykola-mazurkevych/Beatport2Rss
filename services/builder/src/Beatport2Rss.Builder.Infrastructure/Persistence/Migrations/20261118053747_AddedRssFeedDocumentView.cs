using Beatport2Rss.Common.EntityFrameworkCore.Extensions;

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Beatport2Rss.Builder.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddedRssFeedDocumentView : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateView<BuilderDbContext>("vwRssFeedDocuments", MigrationDirection.Up);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateView<BuilderDbContext>("vwRssFeedDocuments", MigrationDirection.Down);
        }
    }
}
