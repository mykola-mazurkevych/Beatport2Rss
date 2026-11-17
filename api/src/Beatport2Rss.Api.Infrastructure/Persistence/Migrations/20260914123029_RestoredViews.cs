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
            migrationBuilder.UpdateView(typeof(ApiDbContext), "vwUsers", ViewOperation.Up);
            migrationBuilder.UpdateView(typeof(ApiDbContext), "vwTags", ViewOperation.Up);
            migrationBuilder.UpdateView(typeof(ApiDbContext), "vwFeeds", ViewOperation.Up);
            migrationBuilder.UpdateView(typeof(ApiDbContext), "vwSessions", ViewOperation.Up);
            migrationBuilder.UpdateView(typeof(ApiDbContext), "vwSubscriptions", ViewOperation.Up);
            migrationBuilder.UpdateView(typeof(ApiDbContext), "vwSubscriptionTags", ViewOperation.Up);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateView(typeof(ApiDbContext), "vwSubscriptionTags", ViewOperation.Down);
            migrationBuilder.UpdateView(typeof(ApiDbContext), "vwSubscriptions", ViewOperation.Down);
            migrationBuilder.UpdateView(typeof(ApiDbContext), "vwSessions", ViewOperation.Down);
            migrationBuilder.UpdateView(typeof(ApiDbContext), "vwFeeds", ViewOperation.Down);
            migrationBuilder.UpdateView(typeof(ApiDbContext), "vwTags", ViewOperation.Down);
            migrationBuilder.UpdateView(typeof(ApiDbContext), "vwUsers", ViewOperation.Down);
        }
    }
}
