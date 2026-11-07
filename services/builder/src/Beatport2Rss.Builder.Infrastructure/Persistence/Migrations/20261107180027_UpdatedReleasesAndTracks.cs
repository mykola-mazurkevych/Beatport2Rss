using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Beatport2Rss.Builder.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedReleasesAndTracks : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Tracks_BeatportId",
                schema: "builder",
                table: "Tracks");

            migrationBuilder.DropColumn(
                name: "BeatportId",
                schema: "builder",
                table: "Tracks");

            migrationBuilder.DropColumn(
                name: "BeatportUri",
                schema: "builder",
                table: "Tracks");

            migrationBuilder.AddColumn<string>(
                name: "BeatportSlug",
                schema: "builder",
                table: "Releases",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false);

            migrationBuilder.CreateIndex(
                name: "IX_Subscriptions_BeatportId",
                schema: "builder",
                table: "Subscriptions",
                column: "BeatportId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Subscriptions_BeatportId",
                schema: "builder",
                table: "Subscriptions");

            migrationBuilder.DropColumn(
                name: "BeatportSlug",
                schema: "builder",
                table: "Releases");

            migrationBuilder.AddColumn<int>(
                name: "BeatportId",
                schema: "builder",
                table: "Tracks",
                type: "integer",
                nullable: false);

            migrationBuilder.AddColumn<string>(
                name: "BeatportUri",
                schema: "builder",
                table: "Tracks",
                type: "character varying(500)",
                maxLength: 500,
                nullable: false);

            migrationBuilder.CreateIndex(
                name: "IX_Tracks_BeatportId",
                schema: "builder",
                table: "Tracks",
                column: "BeatportId",
                unique: true);
        }
    }
}
