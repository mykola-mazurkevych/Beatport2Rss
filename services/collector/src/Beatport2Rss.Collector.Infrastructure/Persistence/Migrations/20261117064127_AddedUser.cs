using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Beatport2Rss.Collector.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddedUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SubscribersCount",
                schema: "collector",
                table: "Subscriptions");

            migrationBuilder.CreateTable(
                name: "Users",
                schema: "collector",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Feeds",
                schema: "collector",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Feeds", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Feeds_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "collector",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FeedSubscriptions",
                schema: "collector",
                columns: table => new
                {
                    FeedId = table.Column<Guid>(type: "uuid", nullable: false),
                    SubscriptionId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FeedSubscriptions", x => new { x.FeedId, x.SubscriptionId });
                    table.ForeignKey(
                        name: "FK_FeedSubscriptions_Feeds_FeedId",
                        column: x => x.FeedId,
                        principalSchema: "collector",
                        principalTable: "Feeds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FeedSubscriptions_Subscriptions_SubscriptionId",
                        column: x => x.SubscriptionId,
                        principalSchema: "collector",
                        principalTable: "Subscriptions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Feeds_UserId",
                schema: "collector",
                table: "Feeds",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_FeedSubscriptions_SubscriptionId",
                schema: "collector",
                table: "FeedSubscriptions",
                column: "SubscriptionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FeedSubscriptions",
                schema: "collector");

            migrationBuilder.DropTable(
                name: "Feeds",
                schema: "collector");

            migrationBuilder.DropTable(
                name: "Users",
                schema: "collector");

            migrationBuilder.AddColumn<int>(
                name: "SubscribersCount",
                schema: "collector",
                table: "Subscriptions",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}
