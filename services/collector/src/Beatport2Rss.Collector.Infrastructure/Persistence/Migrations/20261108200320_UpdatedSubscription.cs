using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Beatport2Rss.Collector.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedSubscription : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BeatportSlug",
                schema: "collector",
                table: "Subscriptions");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BeatportSlug",
                schema: "collector",
                table: "Subscriptions",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");
        }
    }
}
