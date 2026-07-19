using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Beatport2Rss.Builder.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "builder");

            migrationBuilder.CreateTable(
                name: "Artists",
                schema: "builder",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    BeatportId = table.Column<int>(type: "integer", nullable: false),
                    BeatportUri = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Artists", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Labels",
                schema: "builder",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    BeatportId = table.Column<int>(type: "integer", nullable: false),
                    BeatportUri = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Labels", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Releases",
                schema: "builder",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    BeatportId = table.Column<int>(type: "integer", nullable: false),
                    BeatportUri = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Name = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    ReleaseDate = table.Column<DateOnly>(type: "date", nullable: false),
                    LabelId = table.Column<Guid>(type: "uuid", nullable: false),
                    CatalogNumber = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    ImageUri = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Releases", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Releases_Labels_LabelId",
                        column: x => x.LabelId,
                        principalSchema: "builder",
                        principalTable: "Labels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ReleaseArtists",
                schema: "builder",
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
                        principalSchema: "builder",
                        principalTable: "Artists",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ReleaseArtists_Releases_ReleaseId",
                        column: x => x.ReleaseId,
                        principalSchema: "builder",
                        principalTable: "Releases",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Tracks",
                schema: "builder",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    BeatportId = table.Column<int>(type: "integer", nullable: false),
                    BeatportUri = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
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
                        principalSchema: "builder",
                        principalTable: "Releases",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TrackArtists",
                schema: "builder",
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
                        principalSchema: "builder",
                        principalTable: "Artists",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TrackArtists_Tracks_TrackId",
                        column: x => x.TrackId,
                        principalSchema: "builder",
                        principalTable: "Tracks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Artists_BeatportId",
                schema: "builder",
                table: "Artists",
                column: "BeatportId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Labels_BeatportId",
                schema: "builder",
                table: "Labels",
                column: "BeatportId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ReleaseArtists_ArtistId",
                schema: "builder",
                table: "ReleaseArtists",
                column: "ArtistId");

            migrationBuilder.CreateIndex(
                name: "IX_Releases_BeatportId",
                schema: "builder",
                table: "Releases",
                column: "BeatportId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Releases_LabelId",
                schema: "builder",
                table: "Releases",
                column: "LabelId");

            migrationBuilder.CreateIndex(
                name: "IX_TrackArtists_ArtistId",
                schema: "builder",
                table: "TrackArtists",
                column: "ArtistId");

            migrationBuilder.CreateIndex(
                name: "IX_Tracks_BeatportId",
                schema: "builder",
                table: "Tracks",
                column: "BeatportId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tracks_ReleaseId",
                schema: "builder",
                table: "Tracks",
                column: "ReleaseId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ReleaseArtists",
                schema: "builder");

            migrationBuilder.DropTable(
                name: "TrackArtists",
                schema: "builder");

            migrationBuilder.DropTable(
                name: "Artists",
                schema: "builder");

            migrationBuilder.DropTable(
                name: "Tracks",
                schema: "builder");

            migrationBuilder.DropTable(
                name: "Releases",
                schema: "builder");

            migrationBuilder.DropTable(
                name: "Labels",
                schema: "builder");
        }
    }
}
