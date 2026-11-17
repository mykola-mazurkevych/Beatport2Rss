using Beatport2Rss.Api.Infrastructure.Persistence;
using Beatport2Rss.Common.EntityFrameworkCore.Extensions;

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
            migrationBuilder.UpdateView<ApiDbContext>("vwUsers", MigrationDirection.Up);
            migrationBuilder.UpdateView<ApiDbContext>("vwTags", MigrationDirection.Up);
            migrationBuilder.UpdateView<ApiDbContext>("vwFeeds", MigrationDirection.Up);
            migrationBuilder.UpdateView<ApiDbContext>("vwSessions", MigrationDirection.Up);
            migrationBuilder.UpdateView<ApiDbContext>("vwSubscriptions", MigrationDirection.Up);
            migrationBuilder.UpdateView<ApiDbContext>("vwSubscriptionTags", MigrationDirection.Up);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateView<ApiDbContext>("vwSubscriptionTags", MigrationDirection.Down);
            migrationBuilder.UpdateView<ApiDbContext>("vwSubscriptions", MigrationDirection.Down);
            migrationBuilder.UpdateView<ApiDbContext>("vwSessions", MigrationDirection.Down);
            migrationBuilder.UpdateView<ApiDbContext>("vwFeeds", MigrationDirection.Down);
            migrationBuilder.UpdateView<ApiDbContext>("vwTags", MigrationDirection.Down);
            migrationBuilder.UpdateView<ApiDbContext>("vwUsers", MigrationDirection.Down);
        }
    }
}
