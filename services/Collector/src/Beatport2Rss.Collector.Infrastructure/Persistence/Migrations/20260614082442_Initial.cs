using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Beatport2Rss.ReleaseCollector.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "collector");

            migrationBuilder.CreateTable(
                name: "Releases",
                schema: "collector",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    BeatportId = table.Column<int>(type: "integer", nullable: false),
                    BeatportSlug = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Name = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    CatalogNumber = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    ImageUri = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    ReleaseDate = table.Column<DateOnly>(type: "date", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Releases", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Subscriptions",
                schema: "collector",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    BeatportType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    BeatportId = table.Column<int>(type: "integer", nullable: false),
                    BeatportSlug = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    SubscribersCount = table.Column<int>(type: "integer", nullable: false),
                    RefreshedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Subscriptions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Tracks",
                schema: "collector",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    BeatportId = table.Column<int>(type: "integer", nullable: false),
                    BeatportSlug = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Number = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    MixName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Length = table.Column<TimeSpan>(type: "interval", nullable: false),
                    SampleUri = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    ReleaseId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tracks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Tracks_Releases_ReleaseId",
                        column: x => x.ReleaseId,
                        principalSchema: "collector",
                        principalTable: "Releases",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ReleaseSubscriptions",
                schema: "collector",
                columns: table => new
                {
                    ReleaseId = table.Column<Guid>(type: "uuid", nullable: false),
                    SubscriptionId = table.Column<Guid>(type: "uuid", nullable: false),
                    Type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReleaseSubscriptions", x => new { x.ReleaseId, x.SubscriptionId, x.Type });
                    table.ForeignKey(
                        name: "FK_ReleaseSubscriptions_Releases_ReleaseId",
                        column: x => x.ReleaseId,
                        principalSchema: "collector",
                        principalTable: "Releases",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ReleaseSubscriptions_Subscriptions_SubscriptionId",
                        column: x => x.SubscriptionId,
                        principalSchema: "collector",
                        principalTable: "Subscriptions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TrackSubscriptions",
                schema: "collector",
                columns: table => new
                {
                    TrackId = table.Column<Guid>(type: "uuid", nullable: false),
                    SubscriptionId = table.Column<Guid>(type: "uuid", nullable: false),
                    Type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrackSubscriptions", x => new { x.TrackId, x.SubscriptionId, x.Type });
                    table.ForeignKey(
                        name: "FK_TrackSubscriptions_Subscriptions_SubscriptionId",
                        column: x => x.SubscriptionId,
                        principalSchema: "collector",
                        principalTable: "Subscriptions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TrackSubscriptions_Tracks_TrackId",
                        column: x => x.TrackId,
                        principalSchema: "collector",
                        principalTable: "Tracks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Releases_BeatportId",
                schema: "collector",
                table: "Releases",
                column: "BeatportId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ReleaseSubscriptions_SubscriptionId",
                schema: "collector",
                table: "ReleaseSubscriptions",
                column: "SubscriptionId");

            migrationBuilder.CreateIndex(
                name: "IX_Subscriptions_BeatportSlug",
                schema: "collector",
                table: "Subscriptions",
                column: "BeatportSlug");

            migrationBuilder.CreateIndex(
                name: "IX_Subscriptions_BeatportType_BeatportId",
                schema: "collector",
                table: "Subscriptions",
                columns: new[] { "BeatportType", "BeatportId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tracks_BeatportId",
                schema: "collector",
                table: "Tracks",
                column: "BeatportId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tracks_ReleaseId",
                schema: "collector",
                table: "Tracks",
                column: "ReleaseId");

            migrationBuilder.CreateIndex(
                name: "IX_TrackSubscriptions_SubscriptionId",
                schema: "collector",
                table: "TrackSubscriptions",
                column: "SubscriptionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ReleaseSubscriptions",
                schema: "collector");

            migrationBuilder.DropTable(
                name: "TrackSubscriptions",
                schema: "collector");

            migrationBuilder.DropTable(
                name: "Subscriptions",
                schema: "collector");

            migrationBuilder.DropTable(
                name: "Tracks",
                schema: "collector");

            migrationBuilder.DropTable(
                name: "Releases",
                schema: "collector");
        }
    }
}
