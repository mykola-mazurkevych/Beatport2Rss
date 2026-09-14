using Beatport2Rss.Api.Infrastructure.Persistence.Extensions;

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Beatport2Rss.Api.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RestoredViews : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateView("vwUsers", MigrationBuilderExtensions.Operation.Up);
            migrationBuilder.UpdateView("vwTags", MigrationBuilderExtensions.Operation.Up);
            migrationBuilder.UpdateView("vwFeeds", MigrationBuilderExtensions.Operation.Up);
            migrationBuilder.UpdateView("vwSessions", MigrationBuilderExtensions.Operation.Up);
            migrationBuilder.UpdateView("vwSubscriptions", MigrationBuilderExtensions.Operation.Up);
            migrationBuilder.UpdateView("vwSubscriptionTags", MigrationBuilderExtensions.Operation.Up);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateView("vwSubscriptionTags", MigrationBuilderExtensions.Operation.Down);
            migrationBuilder.UpdateView("vwSubscriptions", MigrationBuilderExtensions.Operation.Down);
            migrationBuilder.UpdateView("vwSessions", MigrationBuilderExtensions.Operation.Down);
            migrationBuilder.UpdateView("vwFeeds", MigrationBuilderExtensions.Operation.Down);
            migrationBuilder.UpdateView("vwTags", MigrationBuilderExtensions.Operation.Down);
            migrationBuilder.UpdateView("vwUsers", MigrationBuilderExtensions.Operation.Down);
        }
    }
}
