using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Beatport2Rss.Api.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedSubscription : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Bio",
                schema: "api",
                table: "Subscriptions",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Website",
                schema: "api",
                table: "Subscriptions",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Bio",
                schema: "api",
                table: "Subscriptions");

            migrationBuilder.DropColumn(
                name: "Website",
                schema: "api",
                table: "Subscriptions");
        }
    }
}
