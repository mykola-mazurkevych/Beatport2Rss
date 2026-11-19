using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Beatport2Rss.Builder.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddedGenres : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "GenreId",
                schema: "builder",
                table: "Tracks",
                type: "uuid",
                nullable: false);

            migrationBuilder.CreateTable(
                name: "Genres",
                schema: "builder",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    BeatportId = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Genres", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Tracks_GenreId",
                schema: "builder",
                table: "Tracks",
                column: "GenreId");

            migrationBuilder.CreateIndex(
                name: "IX_Genres_BeatportId",
                schema: "builder",
                table: "Genres",
                column: "BeatportId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Genres_Name",
                schema: "builder",
                table: "Genres",
                column: "Name",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Tracks_Genres_GenreId",
                schema: "builder",
                table: "Tracks",
                column: "GenreId",
                principalSchema: "builder",
                principalTable: "Genres",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tracks_Genres_GenreId",
                schema: "builder",
                table: "Tracks");

            migrationBuilder.DropTable(
                name: "Genres",
                schema: "builder");

            migrationBuilder.DropIndex(
                name: "IX_Tracks_GenreId",
                schema: "builder",
                table: "Tracks");

            migrationBuilder.DropColumn(
                name: "GenreId",
                schema: "builder",
                table: "Tracks");
        }
    }
}
