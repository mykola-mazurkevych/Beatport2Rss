using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Beatport2Rss.ReleaseCollector.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class SplitSubscription : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.AddColumn<Guid>(
                name: "LabelId",
                schema: "collector",
                table: "Releases",
                type: "uuid",
                nullable: false);

            migrationBuilder.CreateTable(
                name: "Artists",
                schema: "collector",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    BeatportId = table.Column<int>(type: "integer", nullable: false),
                    BeatportSlug = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    SubscribersCount = table.Column<int>(type: "integer", nullable: false),
                    RefreshedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Artists", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Labels",
                schema: "collector",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    BeatportId = table.Column<int>(type: "integer", nullable: false),
                    BeatportSlug = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    SubscribersCount = table.Column<int>(type: "integer", nullable: false),
                    RefreshedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Labels", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ReleaseArtists",
                schema: "collector",
                columns: table => new
                {
                    ReleaseId = table.Column<Guid>(type: "uuid", nullable: false),
                    ArtistId = table.Column<Guid>(type: "uuid", nullable: false),
                    Type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReleaseArtists", x => new { x.ReleaseId, x.ArtistId, x.Type });
                    table.ForeignKey(
                        name: "FK_ReleaseArtists_Artists_ArtistId",
                        column: x => x.ArtistId,
                        principalSchema: "collector",
                        principalTable: "Artists",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ReleaseArtists_Releases_ReleaseId",
                        column: x => x.ReleaseId,
                        principalSchema: "collector",
                        principalTable: "Releases",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TrackArtists",
                schema: "collector",
                columns: table => new
                {
                    TrackId = table.Column<Guid>(type: "uuid", nullable: false),
                    ArtistId = table.Column<Guid>(type: "uuid", nullable: false),
                    Type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrackArtists", x => new { x.TrackId, x.ArtistId, x.Type });
                    table.ForeignKey(
                        name: "FK_TrackArtists_Artists_ArtistId",
                        column: x => x.ArtistId,
                        principalSchema: "collector",
                        principalTable: "Artists",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TrackArtists_Tracks_TrackId",
                        column: x => x.TrackId,
                        principalSchema: "collector",
                        principalTable: "Tracks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Releases_LabelId",
                schema: "collector",
                table: "Releases",
                column: "LabelId");

            migrationBuilder.CreateIndex(
                name: "IX_Artists_BeatportId",
                schema: "collector",
                table: "Artists",
                column: "BeatportId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Artists_BeatportSlug",
                schema: "collector",
                table: "Artists",
                column: "BeatportSlug");

            migrationBuilder.CreateIndex(
                name: "IX_Labels_BeatportId",
                schema: "collector",
                table: "Labels",
                column: "BeatportId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Labels_BeatportSlug",
                schema: "collector",
                table: "Labels",
                column: "BeatportSlug");

            migrationBuilder.CreateIndex(
                name: "IX_ReleaseArtists_ArtistId",
                schema: "collector",
                table: "ReleaseArtists",
                column: "ArtistId");

            migrationBuilder.CreateIndex(
                name: "IX_TrackArtists_ArtistId",
                schema: "collector",
                table: "TrackArtists",
                column: "ArtistId");

            migrationBuilder.AddForeignKey(
                name: "FK_Releases_Labels_LabelId",
                schema: "collector",
                table: "Releases",
                column: "LabelId",
                principalSchema: "collector",
                principalTable: "Labels",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Releases_Labels_LabelId",
                schema: "collector",
                table: "Releases");

            migrationBuilder.DropTable(
                name: "Labels",
                schema: "collector");

            migrationBuilder.DropTable(
                name: "ReleaseArtists",
                schema: "collector");

            migrationBuilder.DropTable(
                name: "TrackArtists",
                schema: "collector");

            migrationBuilder.DropTable(
                name: "Artists",
                schema: "collector");

            migrationBuilder.DropIndex(
                name: "IX_Releases_LabelId",
                schema: "collector",
                table: "Releases");

            migrationBuilder.DropColumn(
                name: "LabelId",
                schema: "collector",
                table: "Releases");

            migrationBuilder.CreateTable(
                name: "Subscriptions",
                schema: "collector",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BeatportId = table.Column<int>(type: "integer", nullable: false),
                    BeatportSlug = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    BeatportType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    RefreshedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    SubscribersCount = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Subscriptions", x => x.Id);
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
                name: "IX_TrackSubscriptions_SubscriptionId",
                schema: "collector",
                table: "TrackSubscriptions",
                column: "SubscriptionId");
        }
    }
}
