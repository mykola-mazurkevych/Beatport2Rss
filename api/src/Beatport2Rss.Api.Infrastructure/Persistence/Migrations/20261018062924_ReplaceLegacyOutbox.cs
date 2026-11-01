using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Beatport2Rss.Api.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ReplaceLegacyOutbox : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1
                        FROM api_outbox."OutboxMessages"
                        WHERE "PublishedAt" IS NULL
                    ) THEN
                        RAISE EXCEPTION 'Drain the API legacy outbox before applying the Wolverine migration.';
                    END IF;
                END $$;
                """);

            migrationBuilder.RenameTable(
                name: "OutboxMessages",
                schema: "api_outbox",
                newName: "LegacyOutboxMessages");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "LegacyOutboxMessages",
                schema: "api_outbox",
                newName: "OutboxMessages");
        }
    }
}
