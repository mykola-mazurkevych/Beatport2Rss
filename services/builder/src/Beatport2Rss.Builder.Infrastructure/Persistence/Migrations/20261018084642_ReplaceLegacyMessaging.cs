using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Beatport2Rss.Builder.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ReplaceLegacyMessaging : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1
                        FROM builder_outbox."OutboxMessages"
                        WHERE "PublishedAt" IS NULL
                    ) OR EXISTS (
                        SELECT 1
                        FROM builder_inbox."InboxMessages"
                        WHERE "ProcessedAt" IS NULL
                    ) THEN
                        RAISE EXCEPTION 'Drain the Builder legacy inbox/outbox before applying the Wolverine migration.';
                    END IF;
                END $$;
                """);

            migrationBuilder.RenameTable(
                name: "InboxMessages",
                schema: "builder_inbox",
                newName: "LegacyInboxMessages");

            migrationBuilder.RenameTable(
                name: "OutboxMessages",
                schema: "builder_outbox",
                newName: "LegacyOutboxMessages");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "LegacyInboxMessages",
                schema: "builder_inbox",
                newName: "InboxMessages");

            migrationBuilder.RenameTable(
                name: "LegacyOutboxMessages",
                schema: "builder_outbox",
                newName: "OutboxMessages");
        }
    }
}
