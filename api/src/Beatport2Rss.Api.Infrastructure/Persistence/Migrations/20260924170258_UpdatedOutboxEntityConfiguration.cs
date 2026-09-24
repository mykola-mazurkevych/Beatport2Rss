using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Beatport2Rss.Api.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedOutboxEntityConfiguration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_OutboxMessages_PublishedAt_OccurredAt",
                schema: "api_outbox",
                table: "OutboxMessages");

            migrationBuilder.CreateIndex(
                name: "IX_OutboxMessages_OccurredAt",
                schema: "api_outbox",
                table: "OutboxMessages",
                column: "OccurredAt");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_OutboxMessages_OccurredAt",
                schema: "api_outbox",
                table: "OutboxMessages");

            migrationBuilder.CreateIndex(
                name: "IX_OutboxMessages_PublishedAt_OccurredAt",
                schema: "api_outbox",
                table: "OutboxMessages",
                columns: new[] { "PublishedAt", "OccurredAt" });
        }
    }
}
