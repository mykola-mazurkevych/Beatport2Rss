using System;

using Beatport2Rss.Infrastructure.Persistence.Extensions;

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Beatport2Rss.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RemovedSubscriptionRefreshedAt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateView("vwSubscriptions", MigrationBuilderExtensions.Operation.Up);

            migrationBuilder.DropColumn(
                name: "RefreshedAt",
                table: "Subscriptions");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "RefreshedAt",
                table: "Subscriptions",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.UpdateView("vwSubscriptions", MigrationBuilderExtensions.Operation.Down);
        }
    }
}
