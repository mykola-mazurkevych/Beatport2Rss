using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Beatport2Rss.Collector.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddedReleases : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateOnly>(
                name: "RefreshedAt",
                schema: "collector",
                table: "Subscriptions",
                type: "date",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "Releases",
                schema: "collector",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    BeatportId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Releases", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Releases_BeatportId",
                schema: "collector",
                table: "Releases",
                column: "BeatportId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Releases",
                schema: "collector");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "RefreshedAt",
                schema: "collector",
                table: "Subscriptions",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateOnly),
                oldType: "date",
                oldNullable: true);
        }
    }
}
